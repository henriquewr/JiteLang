using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
using JiteLang.Main.LangParser.SyntaxNodes.Statements;
using JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration;
using JiteLang.Main.LangParser.SyntaxTree;
using JiteLang.Main.LangParser.Types;
using JiteLang.Main.LangParser.Types.Predefined;
using JiteLang.Main.Visitor.Syntax;
using JiteLang.Main.Visitor.Type;
using JiteLang.Syntax;

namespace JiteLang.Main.Visitor
{
    internal class TypeCheckerVisitor
    {
        private readonly ITypeVisitor _typeVisitor;

        public TypeCheckerVisitor(ITypeVisitor typeVisitor)
        {
            _typeVisitor = typeVisitor;
        }

        public virtual void CheckNamespaceDeclaration(NamespaceDeclarationSyntax namespaceDeclarationSyntax)
        {
            foreach (var item in namespaceDeclarationSyntax.Body.Members)
            {
                CheckClassDeclaration(item);
            }
        }

        public virtual void CheckClassDeclaration(ClassDeclarationSyntax classDeclarationSyntax)
        {
            CheckBlock(classDeclarationSyntax.Body);
        }

        private void CheckBlock<TMembers>(BlockStatement<TMembers> blockStatement) where TMembers : SyntaxNode
        {
            foreach (var item in blockStatement.Members)
            {
                switch (item.Kind)
                {
                    case SyntaxKind.VariableDeclaration:
                        CheckVariableDeclaration((item as VariableDeclarationSyntax)!);
                        break;
                    case SyntaxKind.MethodDeclaration:
                        CheckMethodDeclaration((item as MethodDeclarationSyntax)!);
                        break;
                    default:
                        throw new UnreachableException();
                }
            }
        }

        public virtual void CheckMethodDeclaration(MethodDeclarationSyntax methodDeclarationSyntax)
        {
            CheckBlock(methodDeclarationSyntax.Body);
        }

        public virtual void CheckVariableDeclaration(VariableDeclarationSyntax variableDeclarationSyntax)
        {
            throw new Exception("");

            //var variableType = (PredefinedTypeSyntax)     variableDeclarationSyntax.Type;

            //var declarationValue = variableDeclarationSyntax.InitialValue;
            //if (declarationValue is not null)
            //{
                
            //    var valType = _typeVisitor.VisitExpression(declarationValue) ?? throw new Exception("Nao é o mesmo tipo");

            //    if (variableType.Keyword.Kind != ((PredefinedTypeSyntax)valType).Keyword.Kind)
            //    {
            //        throw new Exception("Nao é o mesmo tipo");
            //    }
            //}
        }
    }
}
