# OpenMod System.Drawing Implementation
Unturned does not ship the System.Drawing.dll file. Hence we need to emulate it by implementing the color related classes here. Any assembly resolves to System.Drawing are then redirected to this assembly.

The implementation is taken from mono and licensed under the terms of the MIT license.
