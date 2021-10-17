# Cubic Texture Format 1.0 (.ctf) File Specification

### Header
| Offset | Size | Name | Description |
| ------ | ---- | ---- | ----------- |
| 0 | 4 |  HeaderIdentifier | Contains the char array "CTF ". |
| 4 | 2 | Version | The version number of the format. (Unsigned) |
| 6 | 4 | Width | The width of the full texture. (Unsigned) |
| 10 | 4 | Height | The height of the full texture. (Unsigned) |
| 14 | 2 | MipLevels | The number of mip levels. (Unsigned) |
| 16 | 1 | Compressed | Whether or not the image is compressed, using the given format version's compression algorithm. |
| 17 | 1 | Locked | Whether or not the texture is openable with the texture editors (easily worked around, however provides a small level of security for the average user)


### Data
The first level's offset is 18. You must keep track of the total byte offset, to be able to load textures correctly.

| Offset | Size | Name | Description |
| ------ | ---- | ---- | ----------- |
| | 2 | MipLevel | The mip level of this file. (Unsigned) |
| | 4 | Width | The width of this mip level. (Unsigned) |
| | 4 | Height | The height of this mip level. (Unsigned) |
| | 8 | DataSize | The total number of bytes the data contains. |
| | DataSize | Data | The data for this mip level. This data is expressed as RGBA (4 bytes per pixel). |