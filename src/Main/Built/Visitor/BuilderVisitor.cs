//using System;
//using System.Diagnostics;
//using JiteLang.Main.Builder;
//using JiteLang.Main.Built.Expressions;
//using JiteLang.Main.Built.Statements.Declaration;
//using JiteLang.Main.LangParser;
//using JiteLang.Main.LangParser.SyntaxNodes.Expressions;
//using JiteLang.Main.LangParser.SyntaxNodes.Statements.Declaration;
//using JiteLang.Syntax;

//namespace JiteLang.Main.Built.Visitor
//{
//    internal class BuilderVisitor : IBuilderVisitor
//    {
//        public BuiltNamespaceDeclaration VisitNamespaceDeclaration(NamespaceDeclarationSyntax namespaceDeclarationSyntax, Context context)
//        {
//            var identifier = new BuiltIdentifierExpression(namespaceDeclarationSyntax.Identifier.Text);
//            var builtNamespace = new BuiltNamespaceDeclaration(identifier);

//            foreach (var classDeclaration in namespaceDeclarationSyntax.Body.Members)
//            {
//                builtNamespace.Body.Members.Add(VisitClassDeclaration(classDeclaration, context));
//            }

//            return builtNamespace;
//        }

//        public virtual BuiltClassDeclaration VisitClassDeclaration(ClassDeclarationSyntax classDeclarationSyntax, Context context)
//        {
//            var identifier = new BuiltIdentifierExpression(classDeclarationSyntax.Identifier.Text);
//            var builtClass = new BuiltClassDeclaration(identifier);

//            var newContext = new Context(context);

//            foreach (var item in classDeclarationSyntax.Body.Members)
//            {
//                switch (item.Kind)
//                {
//                    case SyntaxKind.ClassDeclaration:
//                        builtClass.Body.Members.Add(VisitClassDeclaration((ClassDeclarationSyntax)item, newContext));
//                        break;
//                    case SyntaxKind.MethodDeclaration:
//                        builtClass.Body.Members.Add(VisitMethodDeclaration((MethodDeclarationSyntax)item, newContext));
//                        break;
//                    case SyntaxKind.VariableDeclaration:
//                        builtClass.Body.Members.Add(VisitVariableDeclaration((VariableDeclarationSyntax)item, newContext));
//                        break;
//                    default:
//                        throw new UnreachableException();
//                }
//            }

//            return builtClass;
//        }

//        public virtual BuiltMethodDeclaration VisitMethodDeclaration(MethodDeclarationSyntax methodDeclarationSyntax, Context context)
//        {
//            var identifier = new BuiltIdentifierExpression(methodDeclarationSyntax.Identifier.Text);
//            var builtMethod = new BuiltMethodDeclaration(identifier);

//            var newContext = new Context(context);

//            foreach (var item in methodDeclarationSyntax.Body.Members)
//            {
//                switch (item.Kind)
//                {
//                    case SyntaxKind.MethodDeclaration:
//                        builtMethod.Body.Members.Add(VisitMethodDeclaration((MethodDeclarationSyntax)item, newContext));
//                        break;
//                    case SyntaxKind.VariableDeclaration:
//                        builtMethod.Body.Members.Add(VisitVariableDeclaration((VariableDeclarationSyntax)item, newContext));
//                        break;
//                    case SyntaxKind.AssignmentExpression:
//                        builtMethod.Body.Members.Add(VisitAssignmentExpression((AssignmentExpressionSyntax)item, newContext));
//                        break;
//                    default:
//                        throw new UnreachableException();
//                }
//            }

//            return builtMethod;
//        }

//        public virtual BuiltExpression VisitExpression(ExpressionSyntax expressionSyntax, Context context)
//        {
//            switch (expressionSyntax.Kind)
//            {
//                case SyntaxKind.LiteralExpression:
//                    return VisitLiteralExpression((LiteralExpressionSyntax)expressionSyntax, context);
//                case SyntaxKind.MemberExpression:
//                    return VisitMemberExpression((MemberExpressionSyntax)expressionSyntax, context);
//                case SyntaxKind.UnaryExpression:
//                    return VisitUnaryExpression((UnaryExpressionSyntax)expressionSyntax, context);
//                case SyntaxKind.CastExpression:
//                    return VisitCastExpression((CastExpressionSyntax)expressionSyntax, context);
//                case SyntaxKind.BinaryExpression:
//                    return VisitBinaryExpression((BinaryExpressionSyntax)expressionSyntax, context);
//                case SyntaxKind.LogicalExpression:
//                    return VisitLogicalExpression((LogicalExpressionSyntax)expressionSyntax, context);
//                case SyntaxKind.IdentifierExpression:
//                    return VisitIdentifierExpression((IdentifierExpressionSyntax)expressionSyntax, context);
//                case SyntaxKind.AssignmentExpression:
//                    return VisitAssignmentExpression((AssignmentExpressionSyntax)expressionSyntax, context);
//                default:
//                    throw new UnreachableException();
//            }
//        }

//        public virtual BuiltAssignmentExpression VisitAssignmentExpression(AssignmentExpressionSyntax assignmentExpressionSyntax, Context context)
//        {
//            var left = VisitExpression(assignmentExpressionSyntax.Left, context);
//            var right = VisitExpression(assignmentExpressionSyntax.Right, context);

//            var builtAssignmentExpr = new BuiltAssignmentExpression(left, assignmentExpressionSyntax.Operator, right);

//            return builtAssignmentExpr;
//        }

//        public virtual BuiltExpression VisitBinaryExpression(BinaryExpressionSyntax binaryExpressionSyntax, Context context)
//        {
//            var left = VisitExpression(binaryExpressionSyntax.Left, context);
//            var right = VisitExpression(binaryExpressionSyntax.Right, context);

//            var builtBinaryExpr = new BuiltBinaryExpression(left, binaryExpressionSyntax.Operation, right);

//            return builtBinaryExpr;
//        }

//        public virtual BuiltExpression VisitCastExpression(CastExpressionSyntax castExpressionSyntax, Context context)
//        {
//            throw new NotImplementedException();
//        }

//        public virtual BuiltExpression VisitIdentifierExpression(IdentifierExpressionSyntax identifierExpressionSyntax, Context context)
//        {
//            var variable = context.Variables[identifierExpressionSyntax.Text];

//            var builtIdentifierExpr = new BuiltIdentifierExpression(identifierExpressionSyntax.Text);

//            throw new NotImplementedException();
//        }

//        public virtual BuiltLiteralExpression VisitLiteralExpression(LiteralExpressionSyntax literalExpressionSyntax, Context context)
//        {
//            var builtLiteral = new BuiltLiteralExpression(literalExpressionSyntax.Value);
//            return builtLiteral;
//        }

//        public virtual BuiltExpression VisitLogicalExpression(LogicalExpressionSyntax logicalExpressionSyntax, Context context)
//        {
//            throw new NotImplementedException();
//        }

//        public virtual BuiltExpression VisitMemberExpression(MemberExpressionSyntax memberExpressionSyntax, Context context)
//        {
//            throw new NotImplementedException();
//        }

//        public virtual BuiltExpression VisitUnaryExpression(UnaryExpressionSyntax unaryExpressionSyntax, Context context)
//        {
//            throw new NotImplementedException();
//        }

//        public virtual BuiltVariableDeclaration VisitVariableDeclaration(VariableDeclarationSyntax variableDeclarationSyntax, Context context)
//        {
//            var identifier = new BuiltIdentifierExpression(variableDeclarationSyntax.Identifier.Text);

//            var builtVariableDeclaration = new BuiltVariableDeclaration(identifier, variableDeclarationSyntax.Type);

//            if (variableDeclarationSyntax.InitialValue is not null)
//            {
//                builtVariableDeclaration.InitialValue = VisitExpression(variableDeclarationSyntax.InitialValue, context);
//            }

//            context.DownStackPosition += 8;
//            context.Variables.Add(identifier.Text, new Context.Variable(context.DownStackPosition, builtVariableDeclaration.Type));

//            return builtVariableDeclaration;
//        }
//    }
//}
