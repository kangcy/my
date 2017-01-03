using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using CommonTools;
using EGT_OTA.Helper;
using System.Drawing;
using Newtonsoft.Json;
using EGT_OTA.Helper.Config;

namespace EGT_OTA.Controllers
{
    /// <summary>
    /// 上传文件
    /// </summary>
    public class UploadController : Controller
    {
        public static string TxtExtensions = ",doc,docx,docm,dotx,txt,xml,htm,html,mhtml,wps,";
        public static string XlsExtensions = ",xls,xlsm,xlsb,xlsm,";
        public static string ImageExtensions = ",jpg,jpeg,jpe,png,gif,bmp,";
        public static string CompressionExtensions = ",zip,rar,";
        public static string AudioExtensions = ",mp3,wav,";
        public static string VideoExtensions = ",mp4,avi,wmv,mkv,3gp,flv,rmvb,";

        public ActionResult UploadFile()
        {
            var result = false;
            var message = string.Empty;
            var count = Request.Files.Count;
            if (count == 0)
            {
                return Json(new { result = result, message = "未上传任何文件" }, JsonRequestBehavior.AllowGet);
            }

            var folder = ZNRequest.GetString("folder");

            var file = Request.Files[0];
            string extension = Path.GetExtension(file.FileName);

            if (string.IsNullOrWhiteSpace(folder))
            {
                folder = "Other";
            }
            else
            {
                if (folder.ToLower() == "pic" && !ImageExtensions.Contains(extension.ToLower().Replace(".", "")))
                {
                    return Json(new { result = false, message = "上传文件格式不正确" }, JsonRequestBehavior.AllowGet);
                }
                if (folder.ToLower() == "music" && !AudioExtensions.Contains(extension.ToLower().Replace(".", "")))
                {
                    return Json(new { result = false, message = "上传文件格式不正确" }, JsonRequestBehavior.AllowGet);
                }
                if (folder.ToLower() == "video" && !VideoExtensions.Contains(extension.ToLower().Replace(".", "")))
                {
                    return Json(new { result = false, message = "上传文件格式不正确" }, JsonRequestBehavior.AllowGet);
                }
            }
            var url = string.Empty;
            try
            {
                string data = DateTime.Now.ToString("yyyy-MM-dd");
                string virtualPath = "~/Upload/" + folder + "/" + data;
                string savePath = this.Server.MapPath(virtualPath);
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                string filename = Path.GetFileName(file.FileName);
                string code = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(10000);
                string fileExtension = Path.GetExtension(filename);//获取文件后缀名(.jpg)
                //filename = code + fileExtension;//重命名文件
                var name = ZNRequest.GetString("name");
                if (string.IsNullOrEmpty(name))
                {
                    filename = code + fileExtension;
                }
                else
                {
                    filename = name + fileExtension;
                }
                filename = filename.Replace("3gp", "mp4");
                savePath = savePath + "\\" + filename;
                file.SaveAs(savePath);
                url = "Upload/" + folder + "/" + data + "/" + filename;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLoger.Error("UploadController_UploadFile" + ex.Message, ex);
                message = ex.Message;
            }
            return Json(new { result = true, message = url }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Upload()
        {
            var error = string.Empty;
            try
            {
                //string data = DateTime.Now.ToString("yyyy-MM-dd");
                //string virtualPath = "Upload/Images/" + data;
                //string savePath = this.Server.MapPath("~/" + virtualPath);
                //if (!Directory.Exists(savePath))
                //{
                //    Directory.CreateDirectory(savePath);
                //}
                //string stream = ZNRequest.GetString("str");
                //stream = stream.IndexOf("data:image/jpeg;base64,") > -1 ? stream.Replace("data:image/jpeg;base64,", "") : stream;
                //string filename = Guid.NewGuid().ToString("N") + ".jpg";
                //savePath = savePath + "\\" + filename;
                //Byte[] streamByte = Convert.FromBase64String(stream);
                //System.IO.MemoryStream ms = new System.IO.MemoryStream(streamByte);
                //Image image = Image.FromStream(ms, true);
                //image.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                //image.Dispose();


                string stream = ZNRequest.GetString("str");
                stream = stream.IndexOf("data:image/jpeg;base64,") > -1 ? stream.Replace("data:image/jpeg;base64,", "") : stream;
                System.IO.MemoryStream ms = new System.IO.MemoryStream(Convert.FromBase64String(stream));


                string random = DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(10000);

                #region  保存缩略图

                string standards = ZNRequest.GetString("standard");///缩略图规格名称
                int isDraw = ZNRequest.GetInt("isDraw");  //是否生成水印
                int isThumb = ZNRequest.GetInt("isThumb"); //是否生成缩略图

                if (isThumb == 1 && !String.IsNullOrEmpty(standards))
                {
                    UploadConfig.ConfigItem config = UploadConfig.Instance.GetConfig(standards);
                    if (config != null)
                    {
                        //缩略图存放根目录
                        string strFile = System.Web.HttpContext.Current.Server.MapPath(config.SavePath) + "/" + DateTime.Now.ToString("yyyyMMdd");
                        if (!Directory.Exists(strFile))
                        {
                            Directory.CreateDirectory(strFile);
                        }
                        Image image = Image.FromStream(ms, true);
                        ///生成缩略图（多种规格的）
                        int i = 0;
                        foreach (UploadConfig.ThumbMode mode in config.ModeList)
                        {
                            ///保存缩略图地址
                            i++;
                            MakeThumbnail(image, mode.Mode, mode.Width, mode.Height, isDraw, strFile + "\\" + random + "_" + i.ToString() + ".jpg");
                        }
                        image.Dispose();
                    }
                }

                #endregion

                //保存原图
                string savePath = System.Web.HttpContext.Current.Server.MapPath("~/Upload/Images/" + standards + "/" + DateTime.Now.ToString("yyyyMMdd") + "/");
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                savePath = savePath + "\\" + random + "_0" + ".jpg";
                Image image2 = Image.FromStream(ms, true);
                //添加水印
                if (isDraw == 1)
                {
                    image2 = WaterMark(image2);
                }
                image2.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                image2.Dispose();

                return Json(new
                {
                    result = true,
                    message = ("Upload/Images/" + standards + "/" + DateTime.Now.ToString("yyyyMMdd") + "/" + random + "_0" + ".jpg")
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                LogHelper.ErrorLoger.Error("UploadController_Upload" + ex.Message, ex);
            }
            return Json(new
            {
                result = false,
                message = error
            }, JsonRequestBehavior.AllowGet);
        }


        #region  生成缩略图
        ///<summary>  
        /// 生成缩略图  
        /// </summary>  
        /// <param name="originalImagePath">源图对象</param>  
        /// <param name="mode">生成缩略图的方式</param>
        /// <param name="width">缩略图宽度</param>  
        /// <param name="height">缩略图高度</param> 
        /// <param name="height">是否添加水印（0：不添加,1：添加）</param>  
        /// <param name="height">缩略图保存路径</param> 
        public void MakeThumbnail(Image originalImage, string mode, int width, int height, int isDraw, string thumbnailPath)
        {
            int towidth = width;
            int toheight = height;
            int x = 0;
            int y = 0;
            int ow = originalImage.Width;//原图宽度
            int oh = originalImage.Height;//原图高度
            switch (mode)
            {
                case "HW"://指定高宽缩放（可能变形）                  
                    break;
                case "W"://指定宽，高按比例                      
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例  
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut"://指定高宽裁减（不变形）                  
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            Image bitmap = new Bitmap(towidth, toheight);//新建一个bmp图片  
            Graphics g = Graphics.FromImage(bitmap);//新建一个画板  
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;//设置高质量插值法  
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;//设置高质量,低速度呈现平滑程度  
            g.Clear(Color.Transparent);//清空画布并以透明背景色填充  
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);//在指定位置并且按指定大小绘制原图片的指定部分  
            try
            {
                ///添加水印
                if (isDraw == 1)
                {
                    bitmap = WaterMark(bitmap);
                }
                //以jpg格式保存缩略图  
                bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                bitmap.Dispose();
                g.Dispose();
            }
        }
        #endregion


        #region 水印

        /// <summary>
        /// 添加水印
        /// </summary>
        /// <param name="bitmap">原始图片</param>
        public Image WaterMark(Image image)
        {
            ///读取水印配置
            CommonConfig.ConfigItem watermarkmodel = CommonConfig.Instance.GetConfig("WaterMark");

            if (watermarkmodel != null)
            {
                if (watermarkmodel.Cate == 1) //判断水印类型
                {
                    //水印图片
                    Image copyImage = Image.FromFile(System.Web.HttpContext.Current.Server.MapPath("/Image/WaterMark/" + watermarkmodel.ImageUrl));
                    int width = 0;
                    int height = 0;
                    switch (watermarkmodel.Location)
                    {
                        case 1: width = 0; height = 0; break;
                        case 2: width = (image.Width - copyImage.Width) / 2; height = 0; break;
                        case 3: width = image.Width - copyImage.Width; height = 0; break;
                        case 4: width = 0; height = (image.Height - copyImage.Height) / 2; break;
                        case 5: width = (image.Width - copyImage.Width) / 2; height = (image.Height - copyImage.Height) / 2; break;
                        case 6: width = image.Width - copyImage.Width; height = (image.Height - copyImage.Height) / 2; break;
                        case 7: width = 0; height = image.Height - copyImage.Height; break;
                        case 8: width = (image.Width - copyImage.Width) / 2; height = image.Height - copyImage.Height; break;
                        case 9: width = image.Width - copyImage.Width; height = image.Height - copyImage.Height; break;
                    }
                    Graphics g = Graphics.FromImage(image);
                    g.DrawImage(copyImage, new Rectangle(width, height, Convert.ToInt16(watermarkmodel.Width), Convert.ToInt16(watermarkmodel.Height)), 0, 0, copyImage.Width, copyImage.Height, GraphicsUnit.Pixel);
                    g.Dispose();
                    copyImage.Dispose();
                }
                else
                {
                    //文字水印
                    int width = 0;
                    int height = 0;
                    int fontwidth = Convert.ToInt32(watermarkmodel.FontSize * watermarkmodel.Word.Length);
                    int fontheight = Convert.ToInt32(watermarkmodel.FontSize);
                    switch (watermarkmodel.Location)
                    {
                        case 1: width = 0; height = 0; break;
                        case 2: width = (image.Width - fontwidth) / 2; height = 0; break;
                        case 3: width = image.Width - fontwidth; height = 0; break;
                        case 4: width = 0; height = (image.Height - fontheight) / 2; break;
                        case 5: width = (image.Width - fontwidth) / 2; height = (image.Height - fontheight) / 2; break;
                        case 6: width = image.Width - fontwidth; height = (image.Height - fontheight) / 2; break;
                        case 7: width = 0; height = image.Height - fontheight; break;
                        case 8: width = (image.Width - fontwidth) / 2; height = image.Height - fontheight; break;
                        case 9: width = image.Width - fontwidth; height = image.Height - fontheight; break;
                    }
                    Graphics g = Graphics.FromImage(image);
                    g.DrawImage(image, 0, 0, image.Width, image.Height);
                    Font f = new Font("Verdana", float.Parse(watermarkmodel.FontSize.ToString()));
                    Brush b = new SolidBrush(Color.White);
                    g.DrawString(watermarkmodel.Word, f, b, width, height);
                    g.Dispose();
                }
            }
            return image;
        }
        #endregion
    }
}
