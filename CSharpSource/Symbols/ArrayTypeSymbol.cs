﻿// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    using Microsoft.CodeAnalysis.CSharp.Symbols.Retargeting;
    using Microsoft.CodeAnalysis.CodeGen;

    /// <summary>
    /// An ArrayTypeSymbol represents an array type, such as int[] or object[,].
    /// </summary>
    internal sealed partial class ArrayTypeSymbol : TypeSymbol, IArrayTypeSymbol
    {
        private readonly TypeSymbol elementType;
        private readonly int rank;
        private readonly NamedTypeSymbol baseType;
        private readonly ImmutableArray<NamedTypeSymbol> interfaces;
        private readonly ImmutableArray<CustomModifier> customModifiers;

        /// <summary>
        /// Create a new ArrayTypeSymbol.
        /// </summary>
        /// <param name="elementType">The element type of this array type.</param>
        /// <param name="customModifiers">Custom modifiers for the element type of this array type.</param>
        /// <param name="rank">The rank of this array type.</param>
        /// <param name="declaringAssembly">The assembly "declaring"/using the array type.</param>
        internal ArrayTypeSymbol(
            AssemblySymbol declaringAssembly,
            TypeSymbol elementType,
            ImmutableArray<CustomModifier> customModifiers = default(ImmutableArray<CustomModifier>),
            int rank = 1)
            : this(elementType,
                   rank,
                   declaringAssembly.GetSpecialType(SpecialType.System_Array),
                   GetArrayInterfaces(elementType, rank, declaringAssembly),
                   customModifiers.NullToEmpty())
        {
        }

        internal ArrayTypeSymbol(
            TypeSymbol elementType,
            int rank,
            NamedTypeSymbol array,
            ImmutableArray<NamedTypeSymbol> constructedInterfaces,
            ImmutableArray<CustomModifier> customModifiers)
        {
            Debug.Assert((object)elementType != null);
            Debug.Assert((object)array != null);
            Debug.Assert(rank >= 1);
            Debug.Assert(constructedInterfaces.Length <= 2);
            Debug.Assert(constructedInterfaces.Length == 0 || rank == 1);
            Debug.Assert(rank == 1 || !customModifiers.Any());

            this.elementType = elementType;
            this.rank = rank;
            this.baseType = array;
            this.interfaces = constructedInterfaces;
            this.customModifiers = customModifiers;
        }

        private static ImmutableArray<NamedTypeSymbol> GetArrayInterfaces(
            TypeSymbol elementType,
            int rank,
            AssemblySymbol declaringAssembly)
        {
            var constructedInterfaces = ArrayBuilder<NamedTypeSymbol>.GetInstance();

            if (rank == 1)
            {
                //There are cases where the platform does contain the interfaces.
                //So it is fine not to have them listed under the type
                var iListOfT = declaringAssembly.GetSpecialType(SpecialType.System_Collections_Generic_IList_T);
                if (!iListOfT.IsErrorType())
                {
                    constructedInterfaces.Add(new ConstructedNamedTypeSymbol(iListOfT, ImmutableArray.Create<TypeSymbol>(elementType)));
                }

                var iReadOnlyListOfT = declaringAssembly.GetSpecialType(SpecialType.System_Collections_Generic_IReadOnlyList_T);

                if (!iReadOnlyListOfT.IsErrorType())
                {
                    constructedInterfaces.Add(new ConstructedNamedTypeSymbol(iReadOnlyListOfT, ImmutableArray.Create<TypeSymbol>(elementType)));
                }
            }

            return constructedInterfaces.ToImmutableAndFree();
        }

        /// <summary>
        /// Gets the list of custom modifiers associated with the array.
        /// Returns an empty list if there are no custom modifiers.
        /// </summary>
        public ImmutableArray<CustomModifier> CustomModifiers
        {
            get
            {
                return customModifiers;
            }
        }

        /// <summary>
        /// Gets the number of dimensions of the array. A regular single-dimensional array
        /// has rank 1, a two-dimensional array has rank 2, etc.
        /// </summary>
        public int Rank
        {
            get
            {
                return rank;
            }
        }

        /// <summary>
        /// Gets the type of the elements stored in the array.
        /// </summary>
        public TypeSymbol ElementType
        {
            get
            {
                return elementType;
            }
        }

        internal override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics
        {
            get
            {
                return baseType;
            }
        }

        internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics
        {
            get
            {
                return interfaces;
            }
        }

        public override bool IsReferenceType
        {
            get
            {
                return true;
            }
        }

        public override bool IsValueType
        {
            get
            {
                return false;
            }
        }

        internal sealed override bool IsManagedType
        {
            get
            {
                return true;
            }
        }

        internal sealed override ObsoleteAttributeData ObsoleteAttributeData
        {
            get { return null; }
        }

        public override ImmutableArray<Symbol> GetMembers()
        {
            return ImmutableArray<Symbol>.Empty;
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            if (name == ".ctor")
            {
                return ImmutableArray.Create<Symbol>(new ArrayConstructor(this));
            }

            if (name == "Set")
            {
                return ImmutableArray.Create<Symbol>(new ArraySetValueMethod(this));
            }

            if (name == "Get")
            {
                return ImmutableArray.Create<Symbol>(new ArrayGetValueMethod(this));
            }

            if (name == "Address")
            {
                return ImmutableArray.Create<Symbol>(new ArrayAddressMethod(this));
            }

            return ImmutableArray<Symbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override SymbolKind Kind
        {
            get
            {
                return SymbolKind.ArrayType;
            }
        }

        public override TypeKind TypeKind
        {
            get
            {
                return TypeKind.ArrayType;
            }
        }

        public override Symbol ContainingSymbol
        {
            get
            {
                return null;
            }
        }

        public override ImmutableArray<Location> Locations
        {
            get
            {
                return ImmutableArray<Location>.Empty;
            }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get
            {
                return ImmutableArray<SyntaxReference>.Empty;
            }
        }

        internal override TResult Accept<TArgument, TResult>(CSharpSymbolVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitArrayType(this, argument);
        }

        public override void Accept(CSharpSymbolVisitor visitor)
        {
            visitor.VisitArrayType(this);
        }

        public override TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor)
        {
            return visitor.VisitArrayType(this);
        }

        internal override bool Equals(TypeSymbol t2, bool ignoreCustomModifiers, bool ignoreDynamic)
        {
            return this.Equals(t2 as ArrayTypeSymbol, ignoreCustomModifiers, ignoreDynamic);
        }

        internal bool Equals(ArrayTypeSymbol other)
        {
            return Equals(other, false, false);
        }

        private bool Equals(ArrayTypeSymbol other, bool ignoreCustomModifiers, bool ignoreDynamic)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if ((object)other == null || other.Rank != Rank || !other.ElementType.Equals(ElementType, ignoreCustomModifiers, ignoreDynamic))
            {
                return false;
            }

            // Make sure custom modifiers are the same.
            if (!ignoreCustomModifiers)
            {
                var mod = this.CustomModifiers;
                var otherMod = other.CustomModifiers;
                var count = mod.Length;

                if (count != otherMod.Length)
                {
                    return false;
                }

                for (int i = 0; i < count; i++)
                {
                    if (!mod[i].Equals(otherMod[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            // We don't want to blow the stack if we have a type like T[][][][][][][][]....[][],
            // so we do not recurse until we have a non-array. Rather, hash all the ranks together
            // and then hash that with the "T" type.

            int hash = 0;
            TypeSymbol current = this;
            while (current.TypeKind == TypeKind.ArrayType)
            {
                var cur = (ArrayTypeSymbol)current;
                hash = Hash.Combine(cur.Rank, hash);
                current = cur.ElementType;
            }

            return Hash.Combine(current, hash);
        }

        public override Accessibility DeclaredAccessibility
        {
            get
            {
                return Accessibility.NotApplicable;
            }
        }

        public override bool IsStatic
        {
            get
            {
                return false;
            }
        }

        public override bool IsAbstract
        {
            get
            {
                return false;
            }
        }

        public override bool IsSealed
        {
            get
            {
                return false;
            }
        }

        #region Use-Site Diagnostics

        internal override DiagnosticInfo GetUseSiteDiagnostic()
        {
            DiagnosticInfo result = null;

            // check element type
            if (DeriveUseSiteDiagnosticFromType(ref result, this.ElementType))
            {
                return result;
            }

            // check custom modifiers
            if (DeriveUseSiteDiagnosticFromCustomModifiers(ref result, this.CustomModifiers))
            {
                return result;
            }

            return result;
        }

        internal override bool GetUnificationUseSiteDiagnosticRecursive(ref DiagnosticInfo result, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
        {
            return elementType.GetUnificationUseSiteDiagnosticRecursive(ref result, owner, ref checkedTypes) ||
                   ((object)baseType != null && baseType.GetUnificationUseSiteDiagnosticRecursive(ref result, owner, ref checkedTypes)) ||
                   GetUnificationUseSiteDiagnosticRecursive(ref result, this.InterfacesNoUseSiteDiagnostics, owner, ref checkedTypes) ||
                   GetUnificationUseSiteDiagnosticRecursive(ref result, this.CustomModifiers, owner, ref checkedTypes);
        }

        #endregion

        #region IArrayTypeSymbol Members

        ITypeSymbol IArrayTypeSymbol.ElementType
        {
            get { return this.ElementType; }
        }

        ImmutableArray<CustomModifier> IArrayTypeSymbol.CustomModifiers
        {
            get { return this.CustomModifiers; }
        }

        bool IArrayTypeSymbol.Equals(IArrayTypeSymbol symbol)
        {
            return this.Equals(symbol as ArrayTypeSymbol);
        }

        #endregion

        #region ISymbol Members

        public override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitArrayType(this);
        }

        public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return visitor.VisitArrayType(this);
        }

        #endregion

        private sealed class ArrayConstructor : SynthesizedInstanceMethodSymbol
        {
            private readonly ImmutableArray<ParameterSymbol> parameters;
            private readonly ArrayTypeSymbol arrayTypeSymbol;

            public ArrayConstructor(ArrayTypeSymbol arrayTypeSymbol)
            {
                this.arrayTypeSymbol = arrayTypeSymbol;
                var intType = arrayTypeSymbol.BaseType.ContainingAssembly.GetSpecialType(SpecialType.System_Int32);
                this.parameters = ImmutableArray.Create<ParameterSymbol>(
                    Enumerable.Range(0, arrayTypeSymbol.Rank).Select(n => new SynthesizedParameterSymbol(this, intType, n, RefKind.None)).ToArray<ParameterSymbol>());
            }

            public override ImmutableArray<ParameterSymbol> Parameters
            {
                get { return parameters; }
            }

            //
            // Consider overriding when implementing a synthesized subclass.
            //

            internal override bool GenerateDebugInfo
            {
                get { return false; }
            }

            public override Accessibility DeclaredAccessibility
            {
                get { return ContainingType.IsAbstract ? Accessibility.Protected : Accessibility.Public; }
            }

            internal override bool IsMetadataFinal()
            {
                return false;
            }

            #region Sealed

            public sealed override Symbol ContainingSymbol
            {
                get
                {
                    return this.arrayTypeSymbol.BaseType;
                }
            }

            public sealed override NamedTypeSymbol ContainingType
            {
                get
                {
                    return this.arrayTypeSymbol.BaseType;
                }
            }

            public sealed override string Name
            {
                get { return WellKnownMemberNames.InstanceConstructorName; }
            }

            internal sealed override bool HasSpecialName
            {
                get { return true; }
            }

            internal sealed override System.Reflection.MethodImplAttributes ImplementationAttributes
            {
                get
                {
                    var containingType = this.arrayTypeSymbol.BaseType;
                    if (containingType.IsComImport)
                    {
                        Debug.Assert(containingType.TypeKind == TypeKind.Class);
                        return System.Reflection.MethodImplAttributes.Runtime | System.Reflection.MethodImplAttributes.InternalCall;
                    }

                    if (containingType.TypeKind == TypeKind.Delegate)
                    {
                        return System.Reflection.MethodImplAttributes.Runtime;
                    }

                    return default(System.Reflection.MethodImplAttributes);
                }
            }

            internal sealed override bool RequiresSecurityObject
            {
                get { return false; }
            }

            public sealed override DllImportData GetDllImportData()
            {
                return null;
            }

            internal sealed override MarshalPseudoCustomAttributeData ReturnValueMarshallingInformation
            {
                get { return null; }
            }

            internal sealed override bool HasDeclarativeSecurity
            {
                get { return false; }
            }

            internal sealed override IEnumerable<Microsoft.Cci.SecurityAttribute> GetSecurityInformation()
            {
                throw ExceptionUtilities.Unreachable;
            }

            internal sealed override ImmutableArray<string> GetAppliedConditionalSymbols()
            {
                return ImmutableArray<string>.Empty;
            }

            public sealed override bool IsVararg
            {
                get { return false; }
            }

            public sealed override ImmutableArray<TypeParameterSymbol> TypeParameters
            {
                get { return ImmutableArray<TypeParameterSymbol>.Empty; }
            }

            internal sealed override LexicalSortKey GetLexicalSortKey()
            {
                //For the sake of matching the metadata output of the native compiler, make synthesized constructors appear last in the metadata.
                //This is not critical, but it makes it easier on tools that are comparing metadata.
                return LexicalSortKey.Last;
            }

            public sealed override ImmutableArray<Location> Locations
            {
                get { return ContainingType.Locations; }
            }

            public sealed override TypeSymbol ReturnType
            {
                get { return this.arrayTypeSymbol.BaseType.ContainingAssembly.GetSpecialType(SpecialType.System_Void); }
            }

            public sealed override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers
            {
                get { return ImmutableArray<CustomModifier>.Empty; }
            }

            public sealed override ImmutableArray<TypeSymbol> TypeArguments
            {
                get { return ImmutableArray<TypeSymbol>.Empty; }
            }

            public sealed override Symbol AssociatedSymbol
            {
                get { return this.arrayTypeSymbol; }
            }

            public sealed override int Arity
            {
                get { return 0; }
            }

            public sealed override bool ReturnsVoid
            {
                get { return true; }
            }

            public sealed override MethodKind MethodKind
            {
                get { return MethodKind.Constructor; }
            }

            public sealed override bool IsExtern
            {
                get
                {
                    // Synthesized constructors of ComImport type are extern
                    NamedTypeSymbol containingType = this.ContainingType;
                    return (object)containingType != null && containingType.IsComImport;
                }
            }

            public sealed override bool IsSealed
            {
                get { return false; }
            }

            public sealed override bool IsAbstract
            {
                get { return false; }
            }

            public sealed override bool IsOverride
            {
                get { return false; }
            }

            public sealed override bool IsVirtual
            {
                get { return false; }
            }

            public sealed override bool IsStatic
            {
                get { return false; }
            }

            public sealed override bool IsAsync
            {
                get { return false; }
            }

            public sealed override bool HidesBaseMethodsByName
            {
                get { return false; }
            }

            internal sealed override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
            {
                return false;
            }

            internal sealed override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
            {
                return false;
            }

            public sealed override bool IsExtensionMethod
            {
                get { return false; }
            }

            internal sealed override Microsoft.Cci.CallingConvention CallingConvention
            {
                get { return Microsoft.Cci.CallingConvention.HasThis; }
            }

            internal sealed override bool IsExplicitInterfaceImplementation
            {
                get { return false; }
            }

            public sealed override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations
            {
                get { return ImmutableArray<MethodSymbol>.Empty; }
            }

            #endregion
        }

        private sealed class ArraySetValueMethod : SynthesizedInstanceMethodSymbol
        {
            private readonly ImmutableArray<ParameterSymbol> parameters;
            private readonly ArrayTypeSymbol arrayTypeSymbol;

            internal ArraySetValueMethod(ArrayTypeSymbol arrayTypeSymbol)
            {
                this.arrayTypeSymbol = arrayTypeSymbol;
                var intType = arrayTypeSymbol.BaseType.ContainingAssembly.GetSpecialType(SpecialType.System_Int32);
                this.parameters = ImmutableArray.Create<ParameterSymbol>(
                    Enumerable.Range(0, arrayTypeSymbol.Rank).Select(n => new SynthesizedParameterSymbol(this, intType, n, RefKind.None))
                    .ToArray<ParameterSymbol>()
                    .Append(new SynthesizedParameterSymbol(this, arrayTypeSymbol.ElementType, arrayTypeSymbol.Rank + 1, RefKind.None)));
            }

            public override ImmutableArray<ParameterSymbol> Parameters
            {
                get { return parameters; }
            }

            #region Sealed

            public sealed override Symbol ContainingSymbol
            {
                get
                {
                    return this.arrayTypeSymbol.BaseType;
                }
            }

            public sealed override NamedTypeSymbol ContainingType
            {
                get
                {
                    return this.arrayTypeSymbol.BaseType;
                }
            }

            public sealed override string Name
            {
                get
                {
                    return "Set";
                }
            }

            internal sealed override bool HasSpecialName
            {
                get { return true; }
            }

            internal sealed override System.Reflection.MethodImplAttributes ImplementationAttributes
            {
                get
                {
                    var containingType = this.arrayTypeSymbol.BaseType;
                    if (containingType.IsComImport)
                    {
                        Debug.Assert(containingType.TypeKind == TypeKind.Class);
                        return System.Reflection.MethodImplAttributes.Runtime | System.Reflection.MethodImplAttributes.InternalCall;
                    }

                    if (containingType.TypeKind == TypeKind.Delegate)
                    {
                        return System.Reflection.MethodImplAttributes.Runtime;
                    }

                    return default(System.Reflection.MethodImplAttributes);
                }
            }

            internal sealed override bool RequiresSecurityObject
            {
                get { return false; }
            }

            public sealed override DllImportData GetDllImportData()
            {
                return null;
            }

            internal sealed override MarshalPseudoCustomAttributeData ReturnValueMarshallingInformation
            {
                get { return null; }
            }

            internal sealed override bool HasDeclarativeSecurity
            {
                get { return false; }
            }

            internal sealed override IEnumerable<Microsoft.Cci.SecurityAttribute> GetSecurityInformation()
            {
                throw ExceptionUtilities.Unreachable;
            }

            internal sealed override ImmutableArray<string> GetAppliedConditionalSymbols()
            {
                return ImmutableArray<string>.Empty;
            }

            public sealed override bool IsVararg
            {
                get { return false; }
            }

            public sealed override ImmutableArray<TypeParameterSymbol> TypeParameters
            {
                get { return ImmutableArray<TypeParameterSymbol>.Empty; }
            }

            internal sealed override LexicalSortKey GetLexicalSortKey()
            {
                //For the sake of matching the metadata output of the native compiler, make synthesized constructors appear last in the metadata.
                //This is not critical, but it makes it easier on tools that are comparing metadata.
                return LexicalSortKey.Last;
            }

            public sealed override ImmutableArray<Location> Locations
            {
                get { return ContainingType.Locations; }
            }

            public sealed override TypeSymbol ReturnType
            {
                get { return this.arrayTypeSymbol.BaseType.ContainingAssembly.GetSpecialType(SpecialType.System_Void); }
            }

            public sealed override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers
            {
                get { return ImmutableArray<CustomModifier>.Empty; }
            }

            public sealed override ImmutableArray<TypeSymbol> TypeArguments
            {
                get { return ImmutableArray<TypeSymbol>.Empty; }
            }

            public sealed override Symbol AssociatedSymbol
            {
                get { return this.arrayTypeSymbol; }
            }

            public sealed override int Arity
            {
                get { return 0; }
            }

            public sealed override bool ReturnsVoid
            {
                get { return true; }
            }

            public sealed override MethodKind MethodKind
            {
                get { return MethodKind.PropertySet; }
            }

            public sealed override bool IsExtern
            {
                get
                {
                    // Synthesized constructors of ComImport type are extern
                    NamedTypeSymbol containingType = this.ContainingType;
                    return (object)containingType != null && containingType.IsComImport;
                }
            }

            public sealed override bool IsSealed
            {
                get { return false; }
            }

            public sealed override bool IsAbstract
            {
                get { return false; }
            }

            public sealed override bool IsOverride
            {
                get { return false; }
            }

            public sealed override bool IsVirtual
            {
                get { return false; }
            }

            public sealed override bool IsStatic
            {
                get { return false; }
            }

            public sealed override bool IsAsync
            {
                get { return false; }
            }

            public sealed override bool HidesBaseMethodsByName
            {
                get { return false; }
            }

            internal sealed override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
            {
                return false;
            }

            internal sealed override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
            {
                return false;
            }

            public sealed override bool IsExtensionMethod
            {
                get { return false; }
            }

            internal sealed override Microsoft.Cci.CallingConvention CallingConvention
            {
                get { return Microsoft.Cci.CallingConvention.HasThis; }
            }

            internal sealed override bool IsExplicitInterfaceImplementation
            {
                get { return false; }
            }

            public sealed override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations
            {
                get { return ImmutableArray<MethodSymbol>.Empty; }
            }

            #endregion

            internal override bool IsMetadataFinal()
            {
                return false;
            }

            internal override bool GenerateDebugInfo
            {
                get { return false; }
            }

            public override Accessibility DeclaredAccessibility
            {
                get { return Accessibility.Public; }
            }
        }

        private sealed class ArrayGetValueMethod : SynthesizedInstanceMethodSymbol
        {
            private readonly ImmutableArray<ParameterSymbol> parameters;
            private readonly ArrayTypeSymbol arrayTypeSymbol;

            internal ArrayGetValueMethod(ArrayTypeSymbol arrayTypeSymbol)
            {
                this.arrayTypeSymbol = arrayTypeSymbol;
                var intType = arrayTypeSymbol.BaseType.ContainingAssembly.GetSpecialType(SpecialType.System_Int32);
                this.parameters = ImmutableArray.Create<ParameterSymbol>(
                    Enumerable.Range(0, arrayTypeSymbol.Rank).Select(n => new SynthesizedParameterSymbol(this, intType, n, RefKind.None))
                    .ToArray<ParameterSymbol>());
            }

            public override ImmutableArray<ParameterSymbol> Parameters
            {
                get { return parameters; }
            }

            #region Sealed

            public sealed override Symbol ContainingSymbol
            {
                get
                {
                    return this.arrayTypeSymbol.BaseType;
                }
            }

            public sealed override NamedTypeSymbol ContainingType
            {
                get
                {
                    return this.arrayTypeSymbol.BaseType;
                }
            }

            public sealed override string Name
            {
                get
                {
                    return "Get";
                }
            }

            internal sealed override bool HasSpecialName
            {
                get { return true; }
            }

            internal sealed override System.Reflection.MethodImplAttributes ImplementationAttributes
            {
                get
                {
                    var containingType = this.arrayTypeSymbol.BaseType;
                    if (containingType.IsComImport)
                    {
                        Debug.Assert(containingType.TypeKind == TypeKind.Class);
                        return System.Reflection.MethodImplAttributes.Runtime | System.Reflection.MethodImplAttributes.InternalCall;
                    }

                    if (containingType.TypeKind == TypeKind.Delegate)
                    {
                        return System.Reflection.MethodImplAttributes.Runtime;
                    }

                    return default(System.Reflection.MethodImplAttributes);
                }
            }

            internal sealed override bool RequiresSecurityObject
            {
                get { return false; }
            }

            public sealed override DllImportData GetDllImportData()
            {
                return null;
            }

            internal sealed override MarshalPseudoCustomAttributeData ReturnValueMarshallingInformation
            {
                get { return null; }
            }

            internal sealed override bool HasDeclarativeSecurity
            {
                get { return false; }
            }

            internal sealed override IEnumerable<Microsoft.Cci.SecurityAttribute> GetSecurityInformation()
            {
                throw ExceptionUtilities.Unreachable;
            }

            internal sealed override ImmutableArray<string> GetAppliedConditionalSymbols()
            {
                return ImmutableArray<string>.Empty;
            }

            public sealed override bool IsVararg
            {
                get { return false; }
            }

            public sealed override ImmutableArray<TypeParameterSymbol> TypeParameters
            {
                get { return ImmutableArray<TypeParameterSymbol>.Empty; }
            }

            internal sealed override LexicalSortKey GetLexicalSortKey()
            {
                //For the sake of matching the metadata output of the native compiler, make synthesized constructors appear last in the metadata.
                //This is not critical, but it makes it easier on tools that are comparing metadata.
                return LexicalSortKey.Last;
            }

            public sealed override ImmutableArray<Location> Locations
            {
                get { return ContainingType.Locations; }
            }

            public sealed override TypeSymbol ReturnType
            {
                get { return this.arrayTypeSymbol.ElementType; }
            }

            public sealed override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers
            {
                get { return ImmutableArray<CustomModifier>.Empty; }
            }

            public sealed override ImmutableArray<TypeSymbol> TypeArguments
            {
                get { return ImmutableArray<TypeSymbol>.Empty; }
            }

            public sealed override Symbol AssociatedSymbol
            {
                get { return this.arrayTypeSymbol; }
            }

            public sealed override int Arity
            {
                get { return 0; }
            }

            public sealed override bool ReturnsVoid
            {
                get { return false; }
            }

            public sealed override MethodKind MethodKind
            {
                get { return MethodKind.PropertyGet; }
            }

            public sealed override bool IsExtern
            {
                get
                {
                    // Synthesized constructors of ComImport type are extern
                    NamedTypeSymbol containingType = this.ContainingType;
                    return (object)containingType != null && containingType.IsComImport;
                }
            }

            public sealed override bool IsSealed
            {
                get { return false; }
            }

            public sealed override bool IsAbstract
            {
                get { return false; }
            }

            public sealed override bool IsOverride
            {
                get { return false; }
            }

            public sealed override bool IsVirtual
            {
                get { return false; }
            }

            public sealed override bool IsStatic
            {
                get { return false; }
            }

            public sealed override bool IsAsync
            {
                get { return false; }
            }

            public sealed override bool HidesBaseMethodsByName
            {
                get { return false; }
            }

            internal sealed override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
            {
                return false;
            }

            internal sealed override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
            {
                return false;
            }

            public sealed override bool IsExtensionMethod
            {
                get { return false; }
            }

            internal sealed override Microsoft.Cci.CallingConvention CallingConvention
            {
                get { return Microsoft.Cci.CallingConvention.HasThis; }
            }

            internal sealed override bool IsExplicitInterfaceImplementation
            {
                get { return false; }
            }

            public sealed override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations
            {
                get { return ImmutableArray<MethodSymbol>.Empty; }
            }

            #endregion

            internal override bool IsMetadataFinal()
            {
                return false;
            }

            internal override bool GenerateDebugInfo
            {
                get { return false; }
            }

            public override Accessibility DeclaredAccessibility
            {
                get { return Accessibility.Public; }
            }
        }

        private sealed class ArrayAddressMethod : SynthesizedInstanceMethodSymbol
        {
            private readonly ImmutableArray<ParameterSymbol> parameters;
            private readonly ArrayTypeSymbol arrayTypeSymbol;

            internal ArrayAddressMethod(ArrayTypeSymbol arrayTypeSymbol)
            {
                this.arrayTypeSymbol = arrayTypeSymbol;
                var intType = arrayTypeSymbol.BaseType.ContainingAssembly.GetSpecialType(SpecialType.System_Int32);
                this.parameters = ImmutableArray.Create<ParameterSymbol>(
                    Enumerable.Range(0, arrayTypeSymbol.Rank).Select(n => new SynthesizedParameterSymbol(this, intType, n, RefKind.None))
                    .ToArray<ParameterSymbol>());
            }

            public override ImmutableArray<ParameterSymbol> Parameters
            {
                get { return parameters; }
            }

            #region Sealed

            public sealed override Symbol ContainingSymbol
            {
                get
                {
                    return this.arrayTypeSymbol.BaseType;
                }
            }

            public sealed override NamedTypeSymbol ContainingType
            {
                get
                {
                    return this.arrayTypeSymbol.BaseType;
                }
            }

            public sealed override string Name
            {
                get
                {
                    return "Address";
                }
            }

            internal sealed override bool HasSpecialName
            {
                get { return true; }
            }

            internal sealed override System.Reflection.MethodImplAttributes ImplementationAttributes
            {
                get
                {
                    var containingType = this.arrayTypeSymbol.BaseType;
                    if (containingType.IsComImport)
                    {
                        Debug.Assert(containingType.TypeKind == TypeKind.Class);
                        return System.Reflection.MethodImplAttributes.Runtime | System.Reflection.MethodImplAttributes.InternalCall;
                    }

                    if (containingType.TypeKind == TypeKind.Delegate)
                    {
                        return System.Reflection.MethodImplAttributes.Runtime;
                    }

                    return default(System.Reflection.MethodImplAttributes);
                }
            }

            internal sealed override bool RequiresSecurityObject
            {
                get { return false; }
            }

            public sealed override DllImportData GetDllImportData()
            {
                return null;
            }

            internal sealed override MarshalPseudoCustomAttributeData ReturnValueMarshallingInformation
            {
                get { return null; }
            }

            internal sealed override bool HasDeclarativeSecurity
            {
                get { return false; }
            }

            internal sealed override IEnumerable<Microsoft.Cci.SecurityAttribute> GetSecurityInformation()
            {
                throw ExceptionUtilities.Unreachable;
            }

            internal sealed override ImmutableArray<string> GetAppliedConditionalSymbols()
            {
                return ImmutableArray<string>.Empty;
            }

            public sealed override bool IsVararg
            {
                get { return false; }
            }

            public sealed override ImmutableArray<TypeParameterSymbol> TypeParameters
            {
                get { return ImmutableArray<TypeParameterSymbol>.Empty; }
            }

            internal sealed override LexicalSortKey GetLexicalSortKey()
            {
                //For the sake of matching the metadata output of the native compiler, make synthesized constructors appear last in the metadata.
                //This is not critical, but it makes it easier on tools that are comparing metadata.
                return LexicalSortKey.Last;
            }

            public sealed override ImmutableArray<Location> Locations
            {
                get { return ContainingType.Locations; }
            }

            public sealed override TypeSymbol ReturnType
            {
                get { return new ByRefReturnErrorTypeSymbol(this.arrayTypeSymbol.ElementType); }
            }

            public sealed override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers
            {
                get { return ImmutableArray<CustomModifier>.Empty; }
            }

            public sealed override ImmutableArray<TypeSymbol> TypeArguments
            {
                get { return ImmutableArray<TypeSymbol>.Empty; }
            }

            public sealed override Symbol AssociatedSymbol
            {
                get { return this.arrayTypeSymbol; }
            }

            public sealed override int Arity
            {
                get { return 0; }
            }

            public sealed override bool ReturnsVoid
            {
                get { return false; }
            }

            public sealed override MethodKind MethodKind
            {
                get { return MethodKind.PropertyGet; }
            }

            public sealed override bool IsExtern
            {
                get
                {
                    // Synthesized constructors of ComImport type are extern
                    NamedTypeSymbol containingType = this.ContainingType;
                    return (object)containingType != null && containingType.IsComImport;
                }
            }

            public sealed override bool IsSealed
            {
                get { return false; }
            }

            public sealed override bool IsAbstract
            {
                get { return false; }
            }

            public sealed override bool IsOverride
            {
                get { return false; }
            }

            public sealed override bool IsVirtual
            {
                get { return false; }
            }

            public sealed override bool IsStatic
            {
                get { return false; }
            }

            public sealed override bool IsAsync
            {
                get { return false; }
            }

            public sealed override bool HidesBaseMethodsByName
            {
                get { return false; }
            }

            internal sealed override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
            {
                return false;
            }

            internal sealed override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
            {
                return false;
            }

            public sealed override bool IsExtensionMethod
            {
                get { return false; }
            }

            internal sealed override Microsoft.Cci.CallingConvention CallingConvention
            {
                get { return Microsoft.Cci.CallingConvention.HasThis; }
            }

            internal sealed override bool IsExplicitInterfaceImplementation
            {
                get { return false; }
            }

            public sealed override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations
            {
                get { return ImmutableArray<MethodSymbol>.Empty; }
            }

            #endregion

            internal override bool IsMetadataFinal()
            {
                return false;
            }

            internal override bool GenerateDebugInfo
            {
                get { return false; }
            }

            public override Accessibility DeclaredAccessibility
            {
                get { return Accessibility.Public; }
            }
        }

    }
}