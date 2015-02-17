// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

/*============================================================
**
**
**
** A wrapper for file handles
**
** 
===========================================================*/
using System;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Versioning;

namespace Microsoft.Win32.SafeHandles
{
    [System.Security.SecurityCritical]  // auto-generated
    internal sealed class SafeFileMappingHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        [System.Security.SecurityCritical]  // auto-generated_required
        internal SafeFileMappingHandle() : base(true) {}

        // 0 is an Invalid Handle
        [System.Security.SecurityCritical]  // auto-generated_required
        internal SafeFileMappingHandle(IntPtr handle, bool ownsHandle) : base (ownsHandle)
        {
            SetHandle(handle);
        }

        [System.Security.SecurityCritical]
        override protected bool ReleaseHandle()
        {
            return Win32Native.CloseHandle(handle);
        }
    }
}

