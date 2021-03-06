﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Trump
{
    ///<summary>
    ///カードの種別（スーツ）を表す列挙子
    ///</summary>
    public enum Suit
    {
        spade = 1,
        heart = 0,
        daiamond = 2,
        club = 3,
        jocker = 4
    }
    /// <summary>
    /// カードの表裏を表す列挙子
    /// </summary>
    public enum Side
    {
        front,
        back
    }
    ///<summary>
    ///トランプ一枚を表すクラス
    ///スーツと数字はコンストラクタ以外で変更不可。
    ///表裏のみゲーム中に変わる。
    ///</summary>


    class Card
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool
        DeleteObject(IntPtr hObject);

        private readonly Suit suit;
        private readonly int number;
        private Side side;
        private readonly string imgURL = "C:/Users/tabuchikenta/source/repos/Trump/Trump/Assets/trump.bmp";
        public static Bitmap originalBitmap; //元画像全体のイメージ
        private Bitmap imageBitmap;
        private System.Windows.Media.ImageSource imageSource;
        public static Bitmap backImageBitmap;//裏面のビットマップ。裏面は共通なのでクラス変数にする。
        /// <summary>
        /// コンストラクタで画像イメージを設定する。
        /// 全絵柄の１枚絵をクラス変数に保持して部分イメージを切り出して割り当てる。
        /// </summary>
        /// <param name="suit"></param>
        /// <param name="number"></param>
        public Card(Suit suit,int number){
            this.suit = suit;
            this.number = number;
            this.side = Side.back;
            const int width = 60;//カード１枚の幅
            const int hidth = 90;//カード１枚の高さ

            if (originalBitmap == null){
                originalBitmap = new Bitmap(480, 630);
                var originalImage = Image.FromFile(imgURL);
                originalBitmap = (Bitmap)originalImage;
                backImageBitmap = ImageRoi(originalBitmap, new Rectangle(7 * width, 6 * hidth, width, hidth));
            }

            int x;
            int y;
          
                if (suit.Equals(Suit.jocker))
                {
                    x = 4;
                    y = 6;
                }
                else {
                    if (number < 8)
                    {
                        x = (int)suit;
                        y = number - 1;
                    }
                    else
                    {  //元画像が８から次の列になっているので読み込み位置をずらす
                        x = (int)suit + 4; 
                        y = number - 8;

                    }
                

            }
            ImageBitmap = ImageRoi(originalBitmap,new Rectangle(x * width,y * hidth,width,hidth));//一枚あたりの大きさ
            IntPtr hbitmap = ImageBitmap.GetHbitmap();
            imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(hbitmap);
        }
        /// <summary>
        /// Bitmapの一部を切り出したBitmapオブジェクトを返す
        /// </summary>
        /// <param name="srcRect">元のBitmapクラスオブジェクト</param>
        /// <param name="roi">切り出す領域</param>
        /// <returns>切り出したBitmapオブジェクト</returns>
        public Bitmap ImageRoi(Bitmap src, Rectangle roi)
        {
            //////////////////////////////////////////////////////////////////////
            // srcRectとroiの重なった領域を取得（画像をはみ出した領域を切り取る）

            // 画像の領域
            var imgRect = new Rectangle(0, 0, src.Width, src.Height);
            // はみ出した部分を切り取る(重なった領域を取得)
            var roiTrim = Rectangle.Intersect(imgRect, roi);
            // 画像の外の領域を指定した場合
            if (roiTrim.IsEmpty == true) return null;

            //////////////////////////////////////////////////////////////////////
            // 画像の切り出し

            // 切り出す大きさと同じサイズのBitmapオブジェクトを作成
            var dst = new Bitmap(roiTrim.Width, roiTrim.Height, src.PixelFormat);
            // BitmapオブジェクトからGraphicsオブジェクトの作成
            var g = Graphics.FromImage(dst);
            // 描画先
            var dstRect = new Rectangle(0, 0, roiTrim.Width, roiTrim.Height);
            // 描画
            g.DrawImage(src, dstRect, roiTrim, GraphicsUnit.Pixel);
            // 解放
            g.Dispose();

            return dst;
        }
        /// <summary>
        /// カードをひっくり返す。
        /// </summary>
        public void ReverseCard() {
            if (side == Side.front)
            {
                side = Side.back;
                IntPtr hbitmap = backImageBitmap.GetHbitmap();
                imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                DeleteObject(hbitmap);
            }
            else {
                side = Side.front;
                IntPtr hbitmap = ImageBitmap.GetHbitmap();
                imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                DeleteObject(hbitmap);
            }
        }

        public int Number { get => number; }
        internal Suit Suit { get => suit; }
        internal Side Side { get => side; set => side = value; }
        public Bitmap ImageBitmap { get => imageBitmap; set => imageBitmap = value; }
        /// <summary>
        /// イメージコントロールのSourceプロパティにイメージを渡すためのハンドル
        /// </summary>
        public ImageSource ImageSource { get => imageSource; set => imageSource = value; }
    }
}
