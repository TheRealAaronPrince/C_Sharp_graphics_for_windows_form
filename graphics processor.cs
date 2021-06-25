using System;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

public class render
{
	//indexed color values for each pixel
	public int[] colorArray = new int[76800];
	//RGBA value array for the pixels
	private byte[] pixelBuffer = new byte[307200];
	//defining the output bitmap (8x larger to reduce bluring when stretching to fill screen)
	public Bitmap output = new Bitmap(2560,1920,PixelFormat.Format32bppRgb);
	private Dictionary<int, Color> colorDict;

	public render()
	{
		colorDict = Enumerable.Range(0, 64)
			.Select(c =>
			{
				return new { c, color = colorTransform(c) };
			})
			.ToDictionary(k => k.c, v => v.color);
	}

	//loop to set a default color for every pixel
	public void clearImg(int color = 0)
	{
		byte red, green, blue;
		colorTransform(color,out red,out green,out blue);
		//number of pixels in the image
		for( int i = 0; i < colorArray.Length; i++)
		{
			//red
			pixelBuffer[(4*i)+0] = red;
			//green
			pixelBuffer[(4*i)+1] = green;
			//blue
			pixelBuffer[(4*i)+2] = blue;
			//alpha (unused for now)
			pixelBuffer[(4*i)+3] = 255;
			colorArray[i] = color;
		}
	}
	//convert color array to pixel buffer
	public void pixelPaint()
	{
		//see clearImg
		for(int i = 0; i < colorArray.Length; i++)
		{
			//0 to width -1
			int x = i % 320;
			//0 to height - 1 (counting only whole number multiples of the width)
			int y = ((i - (i % 320)) / 320);
			singlePixel(x,y,colorArray[((y*320)+x)]);
		}
	}
	//defining the color for an individual pixel
	public void singlePixel(int X, int Y, int color)
	{
		//use -1 for transparent color
		if(color != -1)
		{
			var r = colorDict[color];
			pixelBuffer[(((Y * 320) + X) * 4) + 0] = r.R;
			pixelBuffer[(((Y * 320) + X) * 4) + 1] = r.G;
			pixelBuffer[(((Y * 320) + X) * 4) + 2] = r.B;
			pixelBuffer[(((Y * 320) + X) * 4) + 3] = 255;
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
		//defining the bmp ready for pixel data
		Bitmap bmp = new Bitmap(320,240, PixelFormat.Format32bppRgb);
		//extra data about bitmap
		BitmapData bmpData = bmp.LockBits(new Rectangle(0,0,320,240),ImageLockMode.ReadWrite,PixelFormat.Format32bppArgb);
		//pointer location in memory of bmp data
		IntPtr ptrFirstPixel = bmpData.Scan0;
		Marshal.Copy(pixelBuffer,0,ptrFirstPixel,307200);
		bmp.UnlockBits(bmpData);
		//enlarging the image to reduce scalling blur
		Graphics gr = Graphics.FromImage((System.Drawing.Image)output);
		gr.InterpolationMode = InterpolationMode.NearestNeighbor;
		gr.PixelOffsetMode = PixelOffsetMode.Half;
		gr.DrawImage(bmp,new Rectangle(0,0,output.Width,output.Height),0,0,320,240,GraphicsUnit.Pixel);
	}
}
