using System;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

public class render
{
	public static int width = 320;
	public static int height = 240;
	//defining the bmp ready for pixel data
	public Bitmap bmp = new Bitmap(width,height, PixelFormat.Format32bppRgb);
	//indexed color values for each pixel
	public int[] colorArray = new int[width*height];
	//RGBA value array for the pixels
	public byte[] pixelBuffer = new byte[width*height*4];
	//defining the output bitmap (8x larger to reduce bluring when stretching to fill screen)
	//public Bitmap output = new Bitmap(width,height,PixelFormat.Format32bppRgb);
	private Color[] colorIndexToColor = Enumerable.Range(0, 64)
			.Select(c => colorTransform(c))
			.ToArray();
	//loop to set a default color for every pixel
	public void clearImg(int color = 0)
	{
		//number of pixels in the image
		for( int i = 0; i < colorArray.Length; i++)
		{
			var r = colorIndexToColor[color];
			pixelBuffer[i + 0] = r.R;
			pixelBuffer[i + 1] = r.G;
			pixelBuffer[i + 2] = r.B;
			pixelBuffer[i + 3] = 255;
		}
	}
	//convert color array to pixel buffer
	public void pixelPaint()
	{
		//see clearImg
		for(int i = 0; i < (colorArray.Length-1)*4; i++)
		{
			//0 to width -1
			int x = i % width;
			//0 to height - 1 (counting only whole number multiples of the width)
			int y = ((i - (i % width)) / width);
			singlePixel(x,y,colorArray[((y*width)+x)]);
		}
	}
	//defining the color for an individual pixel
	public void singlePixel(int X, int Y, int color)
	{
		//use -1 for transparent color
		if(color != -1)
		{
			var r = colorIndexToColor[color];
			pixelBuffer[(((Y * width) + X) * 4) + 0] = r.R;
			pixelBuffer[(((Y * width) + X) * 4) + 1] = r.G;
			pixelBuffer[(((Y * width) + X) * 4) + 2] = r.B;
			pixelBuffer[(((Y * width) + X) * 4) + 3] = 255;
		}
	}
	//converting an index into an RGB value
	private static Color colorTransform(int index)
	{
		int red, grn, blu;
		//color index can range from 0 to 63
		if (index > 63 || index < 0)
		{
			return Color.Black;
		}
		//upper two bits (of a 6 bit number)
		red = (index & 48) / 16;
		//middle two bits
		grn = (index & 12) / 4;
		//bottom two bits
		blu = index & 3;
		return Color.FromArgb(red * 85, grn * 85, blu * 85);
	}
	//converting the array to an image
	public void generate()
	{
		//write data direct to allocated ram
		BitmapData bmpData = bmp.LockBits(new Rectangle(0,0,width,height),ImageLockMode.ReadWrite,PixelFormat.Format32bppArgb);
		IntPtr ptrFirstPixel = bmpData.Scan0;
		Marshal.Copy(pixelBuffer,0,ptrFirstPixel,width*height*4);
		bmp.UnlockBits(bmpData);
	}
}
