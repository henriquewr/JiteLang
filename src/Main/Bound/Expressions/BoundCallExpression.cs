using JiteLang.Main.Bound.Statements.Declaration;
using System.Collections.Generic;
using System.Diagnostics;

namespace JiteLang.Main.Bound.Expressions
{
    internal class BoundCallExpression : BoundExpression
    {
        public override BoundKind Kind => BoundKind.CallExpression;
        public BoundCallExpression(BoundNode parent, BoundExpression caller, List<BoundExpression> args) : base(parent)
        {
            Caller = caller;
            Args = args;
            Position = caller.Position;
        }

        public BoundCallExpression(BoundNode parent, BoundExpression caller) : this(parent, caller, new())
        {
        }

        public BoundExpression Caller { get; set; }
        public List<BoundExpression> Args { get; set; }


        //public BoundMethodDeclaration GetMethod()
        //{
        //    if(Caller.Kind == BoundKind.IdentifierExpression)
        //    {
        //        var currentParent = this.Parent;

        //        while (currentParent is not null)
        //        {
        //            if(currentParent.Kind == BoundKind.ClassDeclaration)
        //            {
        //                var parentClass = (BoundClassDeclaration)currentParent;

        //                parentClass.Body.Variables
        //            }

        //            currentParent = currentParent?.Parent;
        //        }
        //    }

        //    throw new UnreachableException();
        //}
    }
}
