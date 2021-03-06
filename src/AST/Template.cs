﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CppSharp.AST
{
    public struct TemplateParameter
    {
        public string Name;
    }

    /// <summary>
    /// The base class of all kinds of template declarations
    /// (e.g., class, function, etc.).
    /// </summary>
    public abstract class Template : Declaration
    {
        protected Template()
        {
            Parameters = new List<TemplateParameter>();
        }

        protected Template(Declaration decl)
        {
            TemplatedDecl = decl;
            Parameters = new List<TemplateParameter>();
        }

        public Declaration TemplatedDecl;

        public List<TemplateParameter> Parameters;

        public override string ToString()
        {
            return TemplatedDecl.ToString();
        }
    }

    /// <summary>
    /// Declaration of a class template.
    /// </summary>
    public class ClassTemplate : Template
    {
        public List<ClassTemplateSpecialization> Specializations;

        public Class TemplatedClass
        {
            get { return TemplatedDecl as Class; }
        }

        public ClassTemplate()
        {
            Specializations = new List<ClassTemplateSpecialization>();
        }

        public ClassTemplate(Declaration decl)
            : base(decl)
        {
            Specializations = new List<ClassTemplateSpecialization>();
        }

        public override T Visit<T>(IDeclVisitor<T> visitor)
        {
            return visitor.VisitClassTemplateDecl(this);
        }

        public override string Name
        {
            get 
            { 
                if(TemplatedDecl != null) 
                    return TemplatedClass.Name;
               return base.Name;
            }
            set 
            { 
                if(TemplatedDecl != null) 
                    TemplatedClass.Name = value;
                else
                    base.Name = value;
            }
        }

        public override string OriginalName
        {
            get 
            { 
                if(TemplatedDecl != null) 
                    return TemplatedClass.OriginalName;
               return base.OriginalName;
            }
            set 
            { 
                if(TemplatedDecl != null) 
                    TemplatedClass.OriginalName = value;
                else
                    base.OriginalName = value;
            }
        }

        public ClassTemplateSpecialization FindSpecialization(
            TemplateSpecializationType type)
        {
            return Specializations.FirstOrDefault(
                spec => spec.Arguments.SequenceEqual(type.Arguments));
        }

        public ClassTemplateSpecialization FindSpecialization(IntPtr ptr)
        {
            return Specializations.FirstOrDefault(spec => spec.OriginalPtr == ptr);
        }

        public ClassTemplatePartialSpecialization FindPartialSpecialization(
            TemplateSpecializationType type)
        {
            return FindSpecialization(type) as ClassTemplatePartialSpecialization;
        }

        public ClassTemplatePartialSpecialization FindPartialSpecialization(IntPtr ptr)
        {
            return FindSpecialization(ptr) as ClassTemplatePartialSpecialization;
        }
    }

    /// <summary>
    /// Describes the kind of template specialization that a particular
    /// template specialization declaration represents.
    /// </summary>
    public enum TemplateSpecializationKind
    {
        /// This template specialization was formed from a template-id but has
        /// not yet been declared, defined, or instantiated.
        Undeclared,

        /// This template specialization was implicitly instantiated from a
        /// template.
        ImplicitInstantiation,

        /// This template specialization was declared or defined by an explicit
        /// specialization or partial specialization.
        ExplicitSpecialization,

        /// This template specialization was instantiated from a template due
        /// to an explicit instantiation declaration request.
        ExplicitInstantiationDeclaration,

        /// This template specialization was instantiated from a template due
        /// to an explicit instantiation definition request.
        ExplicitInstantiationDefinition
    }

    /// <summary>
    /// Represents a class template specialization, which refers to a class
    /// template with a given set of template arguments.
    /// </summary>
    public class ClassTemplateSpecialization : Class
    {
        public  ClassTemplate TemplatedDecl;

        public List<TemplateArgument> Arguments;

        public TemplateSpecializationKind SpecializationKind;

        public ClassTemplateSpecialization()
        {
            Arguments = new List<TemplateArgument>();
        }
    }

    /// <summary>
    /// Represents a class template partial specialization, which refers to
    /// a class template with a given partial set of template arguments.
    /// </summary>
    public class ClassTemplatePartialSpecialization : ClassTemplateSpecialization
    {
    }

    /// <summary>
    /// Declaration of a template function.
    /// </summary>
    public class FunctionTemplate : Template
    {
        public FunctionTemplate(Declaration decl)
            : base(decl)
        {
        }

        public Function TemplatedFunction
        {
            get { return TemplatedDecl as Function; }
        }

        public override T Visit<T>(IDeclVisitor<T> visitor)
        {
            return visitor.VisitFunctionTemplateDecl(this);
        }
    }
}
