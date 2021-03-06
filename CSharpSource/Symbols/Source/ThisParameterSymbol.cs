// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class ThisParameterSymbol : ParameterSymbol
    {
        internal const string SymbolName = "this";

        private readonly MethodSymbol containingMethod;
        private readonly TypeSymbol containingType;

        internal ThisParameterSymbol(MethodSymbol forMethod) : this(forMethod, forMethod.ContainingType)
        {
        }
        internal ThisParameterSymbol(MethodSymbol forMethod, TypeSymbol containingType)
        {
            this.containingMethod = forMethod;
            this.containingType = containingType;
        }

        public override string Name
        {
            get { return SymbolName; }
        }

        public override TypeSymbol Type
        {
            get { return containingType; }
        }

        public override RefKind RefKind
        {
            get
            {
                return
                    ((object)containingType == null || containingType.TypeKind != TypeKind.Struct) ? RefKind.None :
                    ((object)containingMethod != null && containingMethod.MethodKind == MethodKind.Constructor) ? RefKind.Out :
                    RefKind.Ref;
            }
        }

        public override ImmutableArray<Location> Locations
        {
            get { return (object)containingMethod != null ? containingMethod.Locations : ImmutableArray<Location>.Empty; }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { return ImmutableArray<SyntaxReference>.Empty; }
        }

        public override Symbol ContainingSymbol
        {
            get { return (Symbol)containingMethod ?? containingType; }
        }

        internal override ConstantValue ExplicitDefaultConstantValue
        {
            get { return null; }
        }

        internal override bool IsMetadataOptional
        {
            get { return false; }
        }

        public override bool IsParams
        {
            get { return false; }
        }

        internal override bool IsIDispatchConstant
        {
            get { return false; }
        }

        internal override bool IsIUnknownConstant
        {
            get { return false; }
        }

        internal override bool IsCallerFilePath
        {
            get { return false; }
        }

        internal override bool IsCallerLineNumber
        {
            get { return false; }
        }

        internal override bool IsCallerMemberName
        {
            get { return false; }
        }

        public override int Ordinal
        {
            get { return -1; }
        }

        public override ImmutableArray<CustomModifier> CustomModifiers
        {
            get { return ImmutableArray<CustomModifier>.Empty; }
        }

        // TODO: structs
        public override bool IsThis
        {
            get { return true; }
        }

        // "this" is never explicitly declared.
        public override bool IsImplicitlyDeclared
        {
            get { return true; }
        }

        internal override bool IsMetadataIn
        {
            get { return false; }
        }

        internal override bool IsMetadataOut
        {
            get { return false; }
        }

        internal override MarshalPseudoCustomAttributeData MarshallingInformation
        {
            get { return null; }
        }

        internal sealed override bool HasByRefBeforeCustomModifiers
        {
            get { return false; }
        }
    }
}
