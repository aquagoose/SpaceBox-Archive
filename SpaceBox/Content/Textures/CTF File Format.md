# The CTF File Format
The CTF File format is a custom RLE-compression based format that allows loading & storing textures with multiple mip levels.

The file header is laid out as such:

| Current Byte | Number of Bytes | Name | Description |
| ------------ | --------------- | ---- | ----------- |
| 0 | 4 | Width | The width of the full-size image. |
| 4 | 4 | Height | The height of the full-size image. |
| 8 | 2 | MipLevels | The number of mipmap-levels for this image |

The data format is layed out as such:

| Current Byte | Number of Bytes | Name | Description |
| ------------ | --------------- | ---- | ----------- |
| 8 + PreviousImageSize * CurrentMipLevel | 4 | Length | The length of the data for this current mip level. |
| 8 + PreviousImageSize * CurrentMipLevel + 4 | 4 | Width | The width of the current mip level texture |
| 8 + PreviousImageSize * CurrentMipLevel + 8 | 4 | Height | The height of the current mip level texture |
| 8 + PreviousImageSize * CurrentMipLevel + 12 | * | Data | The data of the image. Read for the given length. |