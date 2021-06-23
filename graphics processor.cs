﻿
using System;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;

public class render
{
	//indexed color values for each pixel
	public int[] colorArray = new int[76800];
	//RGBA value array for the pixels
	private byte[] pixelBuffer = new byte[307200];
	//defining the bmp ready for pixel data
	private Bitmap bmp = new Bitmap(320,240, PixelFormat.Format32bppRgb);
	//defining the output bitmap (8x larger to reduce bluring when stretching to fill screen)
	public Bitmap output = new Bitmap(2560,1920,PixelFormat.Format32bppRgb);
	//loop to set a default color for every pixel
	public void clearImg()
	{
		//number of pixels in the image
		for( int i = 0; i < 76800; i++)
		{
			//red
			pixelBuffer[(4*i)+0] = 0;
			//green
			pixelBuffer[(4*i)+1] = 32;
			//blue
			pixelBuffer[(4*i)+2] = 128;
			//alpha (unused for now)
			pixelBuffer[(4*i)+3] = 255;
		}
	}
	//convert color array to pixel buffer
	public void pixelPaint()
	{
		//see clearImg
		for(int i = 0; i < 76800; i++)
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
		byte red, green, blue;
		colorTransform(color,out red,out green,out blue);
		pixelBuffer[(((Y * 320)+ X)*4)+0] = red;
		pixelBuffer[(((Y * 320)+ X)*4)+1] = green;
		pixelBuffer[(((Y * 320)+ X)*4)+2] = blue;
		pixelBuffer[(((Y * 320)+ X)*4)+3] = 255;
	}
	//converting an index into an RGB value
	private void colorTransform(int index, out byte R, out byte G, out byte B)
	{
		int red, grn, blu;
		//color index can range from 0 to 63
		if(index > 63 || index < 0)
		{
			R = 0;
			G = 0;
			B = 0;
		}
		else
		{
			//upper two bits (of a 6 bit number)
			red = (index & 48)/16;
			//middle two bits
			grn = (index & 12)/4;
			//bottom two bits
			blu = index & 3;
			//conveniently, 85*3 = 255
			R = Convert.ToByte(red * 85);
			G = Convert.ToByte(grn * 85);
			B = Convert.ToByte(blu * 85);
		}
	}
	//converting the array to an image
	public void generate()
	{
		//see clearImg
		for(int i = 0; i < 76800; i++)
		{
			//see pixelPaint
			int x = i % 320;
			int y = ((i - (i % 320)) / 320);
			//testing if a pixel is different to the array(for optimisation)
			if(bmp.GetPixel(x,y)!= Color.FromArgb(255, pixelBuffer[(((y * 320)+ x)*4)], pixelBuffer[(((y * 320)+ x)*4)+1], pixelBuffer[(((y * 320)+ x)*4)+2]))
			{
				//setting the pixels to the ARGB color
				bmp.SetPixel(x ,y ,Color.FromArgb(255, pixelBuffer[(((y * 320)+ x)*4)], pixelBuffer[(((y * 320)+ x)*4)+1], pixelBuffer[(((y * 320)+ x)*4)+2]));
			}
		}
		//enlarging the image to reduce scalling blur
		Graphics gr = Graphics.FromImage((System.Drawing.Image)output);
		gr.InterpolationMode = InterpolationMode.NearestNeighbor;
		gr.PixelOffsetMode = PixelOffsetMode.Half;
		gr.DrawImage(bmp,new Rectangle(0,0,output.Width,output.Height),0,0,320,240,GraphicsUnit.Pixel);
	}
}
