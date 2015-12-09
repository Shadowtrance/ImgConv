# ImgConv
Image Converter / Viewer mainly for 3ds but can be used for other images as well.

Convert basically any images to any type (bitmap (.bmp), Gif (.gif non-animated), Icon (.icn), Jpeg (.jpg), Png (.png), Tif/Tiff (.tif) and .bin.

The .bin options (single and batch) are used specifically for converting images for use with 3ds as the images are converted to bgr and rotated 90 degrees to suit the 3ds screens. 

The "single" bin option will (should) convert any image type to .bin, the "batch" bin option will only convert .png to .bin at the moment.

We have four picture boxes available to view images...

1. Combined images (on the left), this is used for screenshots taken on the 3ds where both screens are combined into one image of 400 x 480 dimensions.

2. Separate images (on the right), this is used for separate images that match the dimensions of the top and bottom screens respetively. Top 400 x 240, Bottom 320 x 240.

3. Small picture box on main form, for viewing any images that don't match the dimensions of the combined and separate images boxes. For example, icons etc.

4. Big Images, hidden. Only displays if the image is bigger than the previous picture boxes. This picture box also has pan and zoom implemented (controls listed on main form).

The picture boxes will display any images except those that are in .bin format which you will be informed of when selecting a .bin image file.

It's as simple as clicking "select folder" at the top and selecting your folder with images in it, using the arrow keys to cycle up and down through the list (or click the filename in the list) and clicking the respective button for the type you want to convert it to.

Converted images will be saved to the "converted" output directory in the sub-folder for the type it was converted to.


Includes convert.exe from ImageMagick-6.9.2.6-portable-Q16-x64 http://www.imagemagick.org/ which is used for the convert to .bin options.
