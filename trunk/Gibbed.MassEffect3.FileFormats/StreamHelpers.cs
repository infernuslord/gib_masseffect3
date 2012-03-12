﻿/* Copyright (c) 2012 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System.IO;
using Gibbed.IO;

namespace Gibbed.MassEffect3.FileFormats
{
    public static class StreamHelpers
    {
        public static FileNameHash ReadFileNameHash(this Stream stream)
        {
            var a = stream.ReadValueU32(Endian.Big);
            var b = stream.ReadValueU32(Endian.Big);
            var c = stream.ReadValueU32(Endian.Big);
            var d = stream.ReadValueU32(Endian.Big);
            return new FileNameHash(a, b, c, d);
        }

        public static void WriteFileNameHash(this Stream stream, FileNameHash hash)
        {
            stream.WriteValueU32(hash.A, Endian.Big);
            stream.WriteValueU32(hash.B, Endian.Big);
            stream.WriteValueU64(hash.C, Endian.Big);
            stream.WriteValueU64(hash.D, Endian.Big);
        }
    }
}