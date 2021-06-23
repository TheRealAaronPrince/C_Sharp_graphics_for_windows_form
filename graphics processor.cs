
using System;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

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
	public class tileGraphics
	{
		//initializing the bmp renderer
		public render render = new render();
		//40 x 30, 8x8 tiles
		private int[] tilemap = new int[4800];
		//the fist dimension is the tile's index, the other two dimensions are Y then X, to make it easier to code by hand
		public string[,,] tileArray;
		//initializing the render class
		public void init()
		{
			//defining the tileset
			this.defTiles();
			//setting the bmp array to a solid color as a blank
			render.clearImg();
			//generating the bmp to then be displayed
			render.generate();
		}
		//defining the character tiles (using the ASCII standard for the text characters)
		private void defTiles(string fore = "f", string back = "b")
		{
			tileArray = new string[,,]
			{
{	//	0	0x0 EDGING
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	1	0x1 EDGING
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,fore,fore,fore},
	{back,back,back,fore,fore,fore,fore,fore},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	2	0x2 EDGING
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	3	0x3 EDGING
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,fore,fore,fore},
	{back,back,back,fore,fore,fore,fore,fore},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	4	0x4 EDGING
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{fore,fore,fore,fore,fore,back,back,back},
	{fore,fore,fore,fore,fore,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	5	0x5 EDGING
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{fore,fore,fore,fore,fore,fore,fore,fore},
	{fore,fore,fore,fore,fore,fore,fore,fore},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	6	0x6 EDGING
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{fore,fore,fore,fore,fore,back,back,back},
	{fore,fore,fore,fore,fore,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	7	0x7 EDGING
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{fore,fore,fore,fore,fore,fore,fore,fore},
	{fore,fore,fore,fore,fore,fore,fore,fore},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	8	0x8 EDGING
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	9	0x9 EDGING
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,fore,fore,fore},
	{back,back,back,fore,fore,fore,fore,fore},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	10	0xA EDGING
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	11	0xB EDGING
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,fore,fore,fore},
	{back,back,back,fore,fore,fore,fore,fore},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	12	0xC EDGING
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{fore,fore,fore,fore,fore,back,back,back},
	{fore,fore,fore,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	13	0xD EDGING
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{fore,fore,fore,fore,fore,fore,fore,fore},
	{fore,fore,fore,fore,fore,fore,fore,fore},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	14	0xE EDGING
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{fore,fore,fore,fore,fore,back,back,back},
	{fore,fore,fore,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	15	0xF EDGING
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{fore,fore,fore,fore,fore,fore,fore,fore},
	{fore,fore,fore,fore,fore,fore,fore,fore},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	16	0x10 chess piece base left
	{back,back,back,back,back,back,fore,fore},
	{back,back,back,back,back,fore,fore,fore},
	{back,back,back,back,back,fore,fore,fore},
	{back,back,back,back,back,fore,fore,fore},
	{back,back,back,back,fore,fore,fore,fore},
	{back,back,fore,fore,fore,fore,fore,fore},
	{back,fore,fore,fore,fore,fore,fore,fore},
	{back,back,back,back,back,back,back,back}
},
{	//	17	0x11 chess piece base right
	{fore,fore,back,back,back,back,back,back},
	{fore,fore,fore,back,back,back,back,back},
	{fore,fore,fore,back,back,back,back,back},
	{fore,fore,fore,back,back,back,back,back},
	{fore,fore,fore,fore,back,back,back,back},
	{fore,fore,fore,fore,fore,fore,back,back},
	{fore,fore,fore,fore,fore,fore,fore,back},
	{back,back,back,back,back,back,back,back}
},
{	//	18	0x12 chess piece pawn left
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,fore,fore},
	{back,back,back,back,back,fore,fore,fore},
	{back,back,back,back,fore,fore,fore,fore},
	{back,back,back,back,fore,fore,fore,fore},
	{back,back,back,back,fore,fore,fore,fore},
	{back,back,back,back,fore,fore,fore,fore},
	{back,back,back,back,back,fore,fore,fore}
},
{	//	19	0x13 chess piece pawn right
	{back,back,back,back,back,back,back,back},
	{fore,fore,back,back,back,back,back,back},
	{fore,fore,fore,back,back,back,back,back},
	{fore,fore,fore,fore,back,back,back,back},
	{fore,fore,fore,fore,back,back,back,back},
	{fore,fore,fore,fore,back,back,back,back},
	{fore,fore,fore,fore,back,back,back,back},
	{fore,fore,fore,back,back,back,back,back}
},
{	//	20	0x14 chess piece rook left
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,back,back,fore},
	{back,back,back,fore,fore,back,back,fore},
	{back,back,back,fore,fore,back,back,fore},
	{back,back,back,fore,fore,fore,fore,fore},
	{back,back,back,fore,fore,fore,fore,fore},
	{back,back,back,back,fore,fore,fore,fore},
	{back,back,back,back,back,back,fore,fore}
},
{	//	21	0x15 chess piece rook right
	{back,back,back,back,back,back,back,back},
	{fore,back,back,fore,fore,back,back,back},
	{fore,back,back,fore,fore,back,back,back},
	{fore,back,back,fore,fore,back,back,back},
	{fore,fore,fore,fore,fore,back,back,back},
	{fore,fore,fore,fore,fore,back,back,back},
	{fore,fore,fore,fore,back,back,back,back},
	{fore,fore,back,back,back,back,back,back}
},
{	//	22	0x16 chess piece bishop left
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,fore,fore},
	{back,back,back,back,back,back,back,fore},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,back,fore,fore,fore,back},
	{back,back,back,back,fore,fore,fore,fore},
	{back,back,back,back,back,fore,fore,fore}
},
{	//	23	0x17 chess piece bishop right
	{back,back,back,back,back,back,back,back},
	{fore,fore,back,back,back,back,back,back},
	{fore,fore,fore,back,back,back,back,back},
	{fore,fore,fore,fore,back,back,back,back},
	{fore,fore,fore,fore,back,back,back,back},
	{fore,fore,fore,fore,back,back,back,back},
	{fore,fore,fore,fore,back,back,back,back},
	{fore,fore,fore,back,back,back,back,back}
},
{	//	24	0x18 chess piece queen left
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,fore},
	{back,back,back,back,back,back,fore,fore},
	{back,back,back,fore,fore,fore,fore,fore},
	{back,back,fore,fore,fore,fore,fore,fore},
	{back,back,back,fore,fore,fore,fore,fore},
	{back,back,back,back,fore,fore,fore,fore},
	{back,back,back,back,back,fore,fore,fore}
},
{	//	25	0x19 chess piece queen right
	{back,back,back,back,back,back,back,back},
	{fore,back,back,back,back,back,back,back},
	{fore,fore,back,back,back,back,back,back},
	{fore,fore,fore,fore,fore,back,back,back},
	{fore,fore,fore,fore,fore,fore,back,back},
	{fore,fore,fore,fore,fore,back,back,back},
	{fore,fore,fore,fore,back,back,back,back},
	{fore,fore,fore,back,back,back,back,back}
},
{	//	26	0x1A chess piece king left
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,fore},
	{back,back,back,fore,fore,fore,back,fore},
	{back,back,fore,back,back,back,fore,fore},
	{back,back,fore,back,back,back,back,fore},
	{back,back,back,fore,fore,back,back,fore},
	{back,back,back,back,back,fore,fore,fore},
	{back,back,back,back,back,back,fore,fore}
},
{	//	27	0x1B chess piece king right
	{back,back,back,back,back,back,back,back},
	{fore,back,back,back,back,back,back,back},
	{fore,back,fore,fore,fore,back,back,back},
	{fore,fore,back,back,back,fore,back,back},
	{fore,back,back,back,back,fore,back,back},
	{fore,back,back,fore,fore,back,back,back},
	{fore,fore,fore,back,back,back,back,back},
	{fore,fore,back,back,back,back,back,back}
},
{	//	28	0x1C chess piece knight left
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,fore,fore},
	{back,back,back,fore,fore,fore,fore,fore},
	{back,back,fore,fore,fore,fore,fore,fore},
	{back,back,fore,fore,fore,fore,fore,fore},
	{back,back,back,fore,fore,fore,fore,fore}
},
{	//	29	0x1D chess piece knight right
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{fore,fore,fore,fore,fore,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{fore,fore,fore,fore,fore,fore,back,back},
	{fore,fore,fore,fore,fore,back,back,back},
	{fore,fore,fore,fore,back,back,back,back}
},
{	//	30	0x1E cursor topLeft
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,fore,back,back,back,back,back,back},
	{back,fore,back,back,back,back,back,back},
	{back,fore,back,back,back,back,back,back},
	{back,fore,back,back,back,back,back,back},
	{back,fore,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	31	0x1F cursor bottomRight
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,fore,back},
	{back,back,back,back,back,back,fore,back},
	{back,back,back,back,back,back,fore,back},
	{back,back,back,back,back,back,fore,back},
	{back,back,back,back,back,back,fore,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,back,back,back,back,back,back,back}
},
{	//	32	0x20	ASCII "SPACE"
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	33	0x21	ASCII !
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	34	0x22	ASCII "
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	35	0x23	ASCII #
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{fore,fore,fore,fore,fore,fore,fore,fore},
	{back,fore,fore,back,back,fore,fore,back},
	{fore,fore,fore,fore,fore,fore,fore,fore},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back}
},
{	//	36	0x24	ASCII $
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,fore,fore,fore,back},
	{back,fore,fore,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,back,back,back,back,fore,fore,back},
	{back,fore,fore,fore,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	37	0x25	ASCII %
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,back,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,back,back,back,fore,fore,back}
},
{	//	38	0x26	ASCII &
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,back,fore,fore,fore,back,back,back},
	{back,fore,fore,back,back,fore,fore,fore},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,fore,fore}
},
{	//	39	0x27	ASCII '
	{back,back,back,back,back,fore,fore,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	40	0x28	ASCII (
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,back,fore,fore,back,back}
},
{	//	41	0x29	ASCII )
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,back,back,back,back}
},
{	//	42	0x2A	ASCII *
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back},
	{fore,fore,fore,fore,fore,fore,fore,fore},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,back,back,back,back,back,back}
},
{	//	43	0x2B	ASCII +
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	44	0x2C	ASCII ,
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,back,back,back,back}
},
{	//	45	0x2D	ASCII -
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	46	0x2E	ASCII .
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	47	0x2F	ASCII /
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,fore,fore},
	{back,back,back,back,back,fore,fore,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,fore,fore,back,back,back,back,back}
},
{	//	48	0x30	ASCII 0
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,fore,fore,fore,back},
	{back,fore,fore,fore,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	49	0x31	ASCII 1
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back}
},
{	//	50	0x32	ASCII 2
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,back,back,back,fore,fore,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back}
},
{	//	51	0x33	ASCII 3
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,back,back,back,fore,fore,back},
	{back,back,back,fore,fore,fore,back,back},
	{back,back,back,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	52	0x34	ASCII 4
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,fore,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,back,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,back,fore,fore,back,back}
},
{	//	53	0x35	ASCII 5
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,back,back},
	{back,back,back,back,back,fore,fore,back},
	{back,back,back,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	54	0x36	ASCII 6
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	55	0x37	ASCII 7
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	56	0x38	ASCII 8
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	57	0x39	ASCII 9
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,fore,back},
	{back,back,back,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	58	0x3A	ASCII :
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	59	0x3B	ASCII ;
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,back,back,back,back}
},
{	//	60	0x3C	ASCII <
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,fore,fore,fore,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,back,fore,fore,fore,back}
},
{	//	61	0x3D	ASCII =
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,back,back,back,back,back,back,back}
},
{	//	62	0x3E	ASCII >
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,back,back,fore,fore,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,fore,fore,fore,back,back,back,back}
},
{	//	63	0x3F	ASCII ?
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,back,back,back,fore,fore,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	64	0x40	ASCII @
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,fore,fore,fore,back},
	{back,fore,fore,back,fore,fore,fore,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,fore,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	65	0x41	ASCII A
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back}
},
{	//	66	0x42	ASCII B
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,fore,fore,fore,back,back}
},
{	//	67	0x43	ASCII C
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	68	0x44	ASCII D
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,back,back,back},
	{back,fore,fore,back,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,fore,fore,back,back},
	{back,fore,fore,fore,fore,back,back,back}
},
{	//	69	0x45	ASCII E
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,fore,fore,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back}
},
{	//	70	0x46	ASCII F
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,fore,fore,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back}
},
{	//	71	0x47	ASCII G
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,fore,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	72	0x48	ASCII H
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back}
},
{	//	73	0x49	ASCII I
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	74	0x4A	ASCII J
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,fore,fore,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,fore,fore,back,fore,fore,back,back},
	{back,back,fore,fore,fore,back,back,back}
},
{	//	75	0x4B	ASCII K
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,fore,fore,back,back},
	{back,fore,fore,fore,fore,back,back,back},
	{back,fore,fore,fore,back,back,back,back},
	{back,fore,fore,fore,fore,back,back,back},
	{back,fore,fore,back,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back}
},
{	//	76	0x4C	ASCII L
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back}
},
{	//	77	0x4D	ASCII M
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,back,fore,fore},
	{back,fore,fore,fore,back,fore,fore,fore},
	{back,fore,fore,fore,fore,fore,fore,fore},
	{back,fore,fore,back,fore,back,fore,fore},
	{back,fore,fore,back,back,back,fore,fore},
	{back,fore,fore,back,back,back,fore,fore},
	{back,fore,fore,back,back,back,fore,fore}
},
{	//	78	0x4E	ASCII N
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,fore,back,fore,fore,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,fore,fore,back,fore,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back}
},
{	//	79	0x4F	ASCII O
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	80	0x50	ASCII P
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back}
},
{	//	81	0x51	ASCII Q
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,back,back,back,back,fore,fore,back}
},
{	//	82	0x52	ASCII R
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,fore,fore,fore,back,back},
	{back,fore,fore,fore,fore,back,back,back},
	{back,fore,fore,back,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back}
},
{	//	83	0x53	ASCII S
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,back,back,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	84	0x54	ASCII T
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	85	0x55	ASCII U
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	86	0x56	ASCII V
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	87	0x57	ASCII W
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,back,fore,fore},
	{back,fore,fore,back,back,back,fore,fore},
	{back,fore,fore,back,back,back,fore,fore},
	{back,fore,fore,back,fore,back,fore,fore},
	{back,fore,fore,fore,fore,fore,fore,fore},
	{back,fore,fore,fore,back,fore,fore,fore},
	{back,fore,fore,back,back,back,fore,fore}
},
{	//	88	0x58	ASCII X
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back}
},
{	//	89	0x59	ASCII Y
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	90	0x5A	ASCII Z
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,back,back,back,back,fore,fore,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back}
},
{	//	91	0x5B	ASCII [
	{back,back,fore,fore,fore,fore,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	92	0x5C	ASCII \
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{fore,fore,back,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,back,back,fore,fore,back}
},
{	//	93	0x5D	ASCII ]
	{back,back,fore,fore,fore,fore,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	94	0x5E	ASCII ^
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	95	0x5F	ASCII _
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back}
},
{	//	96	0x60	ASCII `
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	97	0x61	ASCII a
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back}
},
{	//	98	0x62	ASCII b
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,fore,fore,fore,back,back}
},
{	//	99	0x63	ASCII c
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	100	0x64	ASCII d
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,back,back,back},
	{back,fore,fore,back,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,fore,fore,back,back},
	{back,fore,fore,fore,fore,back,back,back}
},
{	//	101	0x65	ASCII e
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,fore,fore,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back}
},
{	//	102	0x66	ASCII f
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,fore,fore,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back}
},
{	//	103	0x67	ASCII g
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,fore,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	104	0x68	ASCII h
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back}
},
{	//	105	0x69	ASCII i
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	106	0x6A	ASCII j
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,fore,fore,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,fore,fore,back,fore,fore,back,back},
	{back,back,fore,fore,fore,back,back,back}
},
{	//	107	0x6B	ASCII k
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,fore,fore,back,back},
	{back,fore,fore,fore,fore,back,back,back},
	{back,fore,fore,fore,back,back,back,back},
	{back,fore,fore,fore,fore,back,back,back},
	{back,fore,fore,back,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back}
},
{	//	108	0x6C	ASCII l
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back}
},
{	//	109	0x6D	ASCII m
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,back,fore,fore},
	{back,fore,fore,fore,back,fore,fore,fore},
	{back,fore,fore,fore,fore,fore,fore,fore},
	{back,fore,fore,back,fore,back,fore,fore},
	{back,fore,fore,back,back,back,fore,fore},
	{back,fore,fore,back,back,back,fore,fore},
	{back,fore,fore,back,back,back,fore,fore}
},
{	//	110	0x6E	ASCII n
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,fore,back,fore,fore,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,fore,fore,back,fore,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back}
},
{	//	111	0x6F	ASCII o
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	112	0x70	ASCII p
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,back,back,back,back,back}
},
{	//	113	0x71	ASCII q
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,back,back,back,back,fore,fore,back}
},
{	//	114	0x72	ASCII r
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,fore,fore,fore,back,back},
	{back,fore,fore,fore,fore,back,back,back},
	{back,fore,fore,back,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back}
},
{	//	115	0x73	ASCII s
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,back,back,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	116	0x74	ASCII t
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	117	0x75	ASCII u
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back}
},
{	//	118	0x76	ASCII v
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	119	0x77	ASCII w
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,back,fore,fore},
	{back,fore,fore,back,back,back,fore,fore},
	{back,fore,fore,back,back,back,fore,fore},
	{back,fore,fore,back,fore,back,fore,fore},
	{back,fore,fore,fore,fore,fore,fore,fore},
	{back,fore,fore,fore,back,fore,fore,fore},
	{back,fore,fore,back,back,back,fore,fore}
},
{	//	120	0x78	ASCII x
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back}
},
{	//	121	0x79	ASCII y
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,fore,fore,back,back,fore,fore,back},
	{back,back,fore,fore,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	122	0x7A	ASCII z
	{back,back,back,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,back,back,back,back,fore,fore,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,fore,fore,back,back,back,back,back},
	{back,fore,fore,fore,fore,fore,fore,back}
},
{	//	123	0x7B	ASCII {
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,back,fore,fore,back,back}
},
{	//	124	0x7C	ASCII |
	{back,back,back,back,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back}
},
{	//	125	0x7D	ASCII }
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,back,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,back,fore,fore,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,back,fore,fore,back,back,back},
	{back,back,fore,fore,back,back,back,back}
},
{	//	126	0x7E	ASCII ~
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,fore,fore,back,back,fore,back},
	{back,fore,fore,fore,fore,fore,fore,back},
	{back,fore,back,back,fore,fore,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	127	0x7F	ASCII "DEL"
	{fore,fore,fore,fore,fore,fore,fore,fore},
	{fore,fore,fore,fore,fore,fore,fore,fore},
	{fore,fore,fore,fore,fore,fore,fore,fore},
	{fore,fore,fore,fore,fore,fore,fore,fore},
	{fore,fore,fore,fore,fore,fore,fore,fore},
	{fore,fore,fore,fore,fore,fore,fore,fore},
	{fore,fore,fore,fore,fore,fore,fore,fore},
	{fore,fore,fore,fore,fore,fore,fore,fore}
},
{	//	128	0x80
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	129	0x81
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	130	0x82
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	131	0x83
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	132	0x84
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	133	0x85
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	134	0x86
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	135	0x87
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	136	0x88
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	137	0x89
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	138	0x8A
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	139	0x8B
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	140	0x8C
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	141	0x8D
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	142	0x8E
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	143	0x8F
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	144	0x90
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	145	0x91
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	146	0x92
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	147	0x93
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	148	0x94
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	149	0x95
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	150	0x96
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	151	0x97
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	152	0x98
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	153	0x99
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	154	0x9A
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	155	0x9B
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	156	0x9C
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	157	0x9D
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	158	0x9E
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	159	0x9F
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	160	0xA0
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	161	0xA1
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	162	0xA2
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	163	0xA3
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	164	0xA4
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	165	0xA5
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	166	0xA6
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	167	0xA7
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	168	0xA8
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	169	0xA9
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	170	0xAA
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	171	0xAB
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	172	0xAC
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	173	0xAD
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	174	0xAE
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	175	0xAF
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	176	0xB0
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	177	0xB1
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	178	0xB2
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	179	0xB3
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	180	0xB4
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	181	0xB5
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	182	0xB6
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	183	0xB7
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	184	0xB8
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	185	0xB9
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	186	0xBA
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	187	0xBB
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	188	0xBC
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	189	0xBD
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	190	0xBE
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	191	0xBF
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	192	0xC0
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	193	0xC1
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	194	0xC2
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	195	0xC3
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	196	0xC4
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	197	0xC5
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	198	0xC6
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	199	0xC7
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	200	0xC8
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	201	0xC9
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	202	0xCA
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	203	0xCB
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	204	0xCC
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	205	0xCD
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	206	0xCE
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	207	0xCF
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	208	0xD0
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	209	0xD1
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	210	0xD2
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	211	0xD3
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	212	0xD4
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	213	0xD5
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	214	0xD6
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	215	0xD7
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	216	0xD8
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	217	0xD9
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	218	0xDA
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	219	0xDB
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	220	0xDC
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	221	0xDD
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	222	0xDE
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	223	0xDF
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	224	0xE0
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	225	0xE1
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	226	0xE2
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	227	0xE3
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	228	0xE4
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	229	0xE5
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	230	0xE6
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	231	0xE7
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	232	0xE8
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	233	0xE9
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	234	0xEA
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	235	0xEB
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	236	0xEC
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	237	0xED
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	238	0xEE
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	239	0xEF
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	240	0xF0
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	241	0xF1
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	242	0xF2
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	243	0xF3
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	244	0xF4
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	245	0xF5
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	246	0xF6
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	247	0xF7
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	248	0xF8
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	249	0xF9
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	250	0xFA
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	251	0xFB
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	252	0xFC
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	253	0xFD
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	254	0xFE
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
{	//	255	0xFF
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back},
	{back,back,back,back,back,back,back,back}
},
			};
		}
		//writing tiles onto the intermediate array
		public void drawTile(int x, int y, int index, int fore = 63, int back = 42)
		{
			//converting x,y to a tile index
			int i = 4 * ((y * 40) + x);
			//testing to see if the tile being written is different to the tile already present
			if(tilemap[i + 0] != index || tilemap[i + 1] != fore || tilemap[i + 2] != back)
			{
				//tile index number
				tilemap[i + 0] = index;
				//foreground color (6 bit indexed RGB)
				tilemap[i + 1] = fore;
				//background color (6 bit indexed RGB)
				tilemap[i + 2] = back;
				//tile changed flag (used for optimisation to prevent redrawing tiles that haven't changed)
				tilemap[i + 3] = 1;
			}
		}
		//this forces the tile at x,y to be redrawn, this can be useful to cover over a sprite that has been drawn directly to the render array
		public void forceUpdateTile(int x, int y)
		{
			//see drawTile
			int i = 4 * ((y * 40) + x);
			//see drawTile
			tilemap[i + 3] = 1;
		}
		//this allows sending a string direct to the tile array in a rectangle
		public void directText(string input, int fore = 63, int back = 0, int startX = 0, int startY = 0, int endX = 39, int endY = 29)
		{
			//converting string to byte array
			byte[] textArray = Encoding.ASCII.GetBytes(input);
			for(int i = 0; i <= textArray.GetUpperBound(0); i++)
			{
				//calculating tile x pos
				int letterX = (i % ((endX + 1) - startX)) + startX;
				//calculating y pos
				int letterY = ((i - i % ((endX  + 1) - startX)) / ((endX  + 1) - startX)) + startY;
				//testing if string excedes bounding box
				if( i == (((endX  + 1) - startX)*((endY  + 1) - startY)))
				{
					break;
				}
				else
				{
					this.drawTile(letterX,letterY, textArray[i],fore,back);
				}
			}
			this.pushTileArray();
		}
		//this uses the first 16 graphics tiles to draw a box on the screen defined by the top left and bottom right corners.
		public void drawBox(int L = 0, int U = 0, int R = 39, int D = 29,int fore = 63, int back = 0)
		{
			int tilei;
			int boxH = (D + 1) - U;
			int boxW = (R + 1) - L;
			for(int i = 0; i < (boxH * boxW); i ++)
			{
				int boxX = i % boxW;
				int boxY = (i - (i % boxW))/boxW;
				//box top left corner
				if(boxX == 0 && boxY == 0)
				{
					tilei = 9;
				}
				//box top right corner
				else if(boxX == (boxW - 1) && boxY == 0)
				{
					tilei = 12;
				}
				//box bottom left corner
				else if(boxX == 0 && boxY == (boxH - 1))
				{
					tilei = 3;
				}
				//box bottom right corner
				else if( boxX == (boxW - 1) && boxY == (boxH - 1))
				{
					tilei = 6;
				}
				//box left and right edges
				else if(boxX == 0 || boxX == (boxW - 1))
				{
					tilei = 10;
				}
				//box top and bottom edges
				else if(boxY == 0 || boxY == (boxH - 1))
				{
					tilei = 5;
				}
				//making the middle of the box empty
				else
				{
					tilei = -1;
				}
				//drawing a line if the width is 1
				if(L == R)
				{
					//top of the line
					if(boxY == 0)
					{
						tilei = 8;
					}
					//middle of the line
					else if(boxY == (boxH - 1))
					{
						tilei = 2;
					}
					//bottom of the line
					else
					{
						tilei = 10;
					}
				}
				//drawing a line if the height is 1
				if(U == D)
				{
					//left of the line
					if(boxX == 0)
					{
						tilei = 1;
					}
					//middle of the line
					else if(boxX == (boxW - 1))
					{
						tilei = 4;
					}
					//right of the line
					else
					{
						tilei = 5;
					}
				}
				//drawing a dot if width and height is 1
				if(L == R && U == D)
				{
					tilei = 0;
				}
				//drawing the tiles to the array
				if(tilei != -1)
				{
					this.drawTile(boxX + L,boxY + U,tilei,fore,back);
				}
			}
		this.pushTileArray();
		}
		//this sets all of the tiles to hex 20, which should be the space character assuming the tileArray hasn't been overwritten
		public void clearTilemap( int fore, int back)
		{
			//number of tiles
			for(int i = 0; i < 1200; i++)
			{
				//see drawTile
				tilemap[4*i+0] = 0x20;
				tilemap[4*i+1] = fore;
				tilemap[4*i+2] = back;
				tilemap[4*i+3] = 1;
			}
			this.pushTileArray();
		}
		//this sends the tile array to the render class to be converted into a BMP format
		public void pushTileArray(bool update = true)
		{
			//see render clearImg
			for(int i = 0; i < 76800; i++)
			{
				//pixels of a tile
				int pixelX = i % 8;
				int pixelY = ((i - (i % 320)) / 320) % 8;
				//tiles' position
				int tileX = ((i % 320) - (i % 8))/8;
				int tileY = (i - (i % 2560))/2560;
				//tilemap position
				int j = (tileY * 40 + tileX)*4;
				//only updating the pixel array if the tile changed flag is set(for optimisation)
				if(tilemap[j+3] == 1)
				{
					//if the pixel of the tile should be the foreground color
					if(tileArray[tilemap[j],pixelY, pixelX] == "f")
					{
						render.colorArray[i] = tilemap[j+1];
					}
					//if the pixel of the tile should be the background color
					else if(tileArray[tilemap[j],pixelY, pixelX] == "b")
					{
						render.colorArray[i] = tilemap[j+2];
					}
					//setting the tile changed flag to zero to indicate the tile has been drawn
					if((pixelX + 1 ) * (pixelY + 1) == 64)
					{
					tilemap[j+3] = 0;
					}
				}
			}
			render.pixelPaint();
			if(update)
			{
				render.generate();
			}
		}
		public void readTileArray(int x, int y, string str, out int val)
		{
			if(str == "index")
			{
				val = tilemap[4*((y*40)+x)];
			}
			else if(str == "fore")
			{
				val = tilemap[4*((y*40)+x)+1];
			}
			else if(str == "back")
			{
				val = tilemap[4*((y*40)+x)+2];
			}
			else
			{
				val = 0;
			}
		}
	}
