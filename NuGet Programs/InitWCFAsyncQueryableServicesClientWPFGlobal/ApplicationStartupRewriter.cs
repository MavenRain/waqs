﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roslyn.Compilers.CSharp;
using RoslynHelper;

namespace InitWCFAsyncQueryableServicesClientWPFGlobal
{
    public class ApplicationStartupRewriter : SyntaxRewriter
    {
        private string _clientContextNamespace;
        private bool _initWCFAsyncQueryableServicesModules;


        public ApplicationStartupRewriter(string clientContextNamespace)
        {
            _clientContextNamespace = clientContextNamespace;
        }

        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            var value = ((CompilationUnitSyntax)base.VisitCompilationUnit(node));
            value = value.AddUsing(_clientContextNamespace);
            value = value.AddUsing(_clientContextNamespace + ".ServiceReference");
            return value;
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            switch (node.Identifier.ValueText)
            {
                case "InitWCFAsyncQueryableServicesModules":
                    _initWCFAsyncQueryableServicesModules = true;
                    break;
            }
            return base.VisitMethodDeclaration(node);
        }

        public override SyntaxNode VisitBlock(BlockSyntax node)
        {
            if (_initWCFAsyncQueryableServicesModules)
            {
                var statements = new[] { "unityContainer.RegisterType<IGlobalWCFService, GlobalWCFServiceClient>(new InjectionConstructor());", "unityContainer.RegisterType<IGlobalClientContext, GlobalClientContext>();" };
                var value = node.InsertStatements(statements.Select(s => Syntax.ParseStatement(s)));
                _initWCFAsyncQueryableServicesModules = false;
                return value;
            }
            return base.VisitBlock(node);
        }
    }
}
