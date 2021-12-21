﻿using Antlr4.Runtime;
using fac.AntlrTools;
using fac.ASTs.Stmts;
using fac.ASTs.Types;
using fac.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fac.ASTs.Structs {
	public class AstTemplateEnumInst: IAst, IAstClass {
		public AstTemplateEnum Class { init; get; }
		public List<IAstType> Templates { init; get; }

		public string FullName { init; get; }
		public string CSharpFullName { get => FullName.Replace ("<", "__lt__").Replace (">", "__gt__").Replace (",", "__comma__"); }
		public List<AstEnumItem> ClassEnumItems { init; get; }
		public List<AstClassVar> ClassVars { init; get; }
		public List<AstClassFunc> ClassFuncs { init; get; }
		private bool m_compiled = false;



		public AstTemplateEnumInst (IToken _token, AstTemplateEnum _class, List<IAstType> _templates, string _fullname) {
			Token = _token;
			Class = _class;
			Templates = _templates;
			FullName = _fullname;
			ClassEnumItems = _class.ClassEnumItems;
			ClassVars = new List<AstClassVar> ();
			foreach (var _var in Class.ClassVars) {
				if (_var.DataType is AstType_Placeholder _phtype) {
					ClassVars.Add (new AstClassVar { Token = _var.Token, Level = _var.Level, Static = _var.Static, DataType = GetImplType (_phtype.Name), Name = _var.Name, DefaultValueRaw = _var.DefaultValueRaw });
				} else {
					ClassVars.Add (_var);
				}
			}
			ClassFuncs = new List<AstClassFunc> ();
			foreach (var _func in Class.ClassFuncs) {
				ClassFuncs.Add (new AstClassFunc (this, _func, GetImplType));
			}
		}

		public IAstType GetImplType (string _ttype_name) {
			for (int i = 0; i < Class.Templates.Count; ++i) {
				if (_ttype_name == Class.Templates[i].Name)
					return Templates[i];
			}
			return null;
		}

		public AstType_Class GetClassType () => AstType_Class.GetType (Token, Class.GetInst (Templates));

		public bool Compile () {
			if (m_compiled)
				return false;
			m_compiled = true;

			Info.CurrentClass = this;
			Info.CurrentFuncVariables = null;

			string _name = FullName[(FullName.LastIndexOf ('.') + 1)..];
			var _sb = new StringBuilder ();
			_sb.Append (@$"public static bool operator== ({_name} _l, {_name} _r) {{
if (_l.@index != _r.@index) {{
	return false;
}}
");
			for (int i = 0; i < ClassEnumItems.Count; ++i) {
				_sb.AppendLine ($" else if (_l.@index == {i}) {{");
				if (ClassEnumItems[i].AttachType == null) {
					_sb.AppendLine ($"return true;");
				} else {
					var _real_var_index = GetRealAttachVarPos (i);
					_sb.AppendLine ($"return _l.{ClassVars[_real_var_index].Name} == _r.{ClassVars[_real_var_index].Name};");
				}
				_sb.AppendLine ($"}}");
			}
			_sb.AppendLine ($" else {{ return false; }}");
			_sb.AppendLine (@$"}}");
			ClassFuncs.Add (Common.ParseCode<AstClassFunc> (_sb.ToString ()));
			ClassFuncs.Add (Common.ParseCode<AstClassFunc> (@$"public static bool operator!= ({_name} _l, {_name} _r) => !(_l == _r);"));

			// Antlr转AST
			foreach (var _var in ClassVars)
				_var.ToAST ();
			foreach (var _func in ClassFuncs)
				_func.ToAST ();

			// 处理AST
			for (int i = 0; i < ExprTraversals.TraversalTypes.Count; ++i) {
				Info.CurrentTraversalType = ExprTraversals.TraversalTypes[i];

				// 类成员变量默认初始化值
				for (int j = 0; j < ClassVars.Count; ++j) {
					if (ClassVars[j].DefaultValue == null)
						continue;
					Info.CurrentFunc = null;
					Info.CurrentFuncVariables = new List<Info.FuncArgumentOrVars> ();
					Info.CurrentFuncVariables.Add (new Info.FuncArgumentOrVars { Group = 0, Vars = new Dictionary<string, AstStmt_DefVariable> () });
					//
					ClassVars[j].DefaultValue = ClassVars[j].DefaultValue.TraversalWrap ((_deep: 0, _group: 0, _loop: i, _cb: ExprTraversals.Traversal));
				}

				// 类成员方法
				for (int j = 0; j < ClassFuncs.Count; ++j) {
					Info.CurrentFunc = ClassFuncs[j];
					Info.CurrentFuncVariables = new List<Info.FuncArgumentOrVars> ();
					Info.CurrentFuncVariables.Add (new Info.FuncArgumentOrVars { Group = 0, ClassFunc = Info.CurrentFunc });
					Info.CurrentFuncVariables.Add (new Info.FuncArgumentOrVars { Group = 1, Vars = new Dictionary<string, AstStmt_DefVariable> () });
					//
					if (i == 2) {
						ClassFuncs[j].BodyCodes.TraversalCalcType ();
						ExprTraversals.Init = ExprTraversals.Complete = true;
						ClassFuncs[j].BodyCodes.TraversalWraps ((_deep: 1, _group: 0, _loop: i, _cb: ExprTraversals.Traversal));
						if (!ExprTraversals.Complete) {
							ExprTraversals.Init = false;
							Info.CurrentFuncVariables = new List<Info.FuncArgumentOrVars> ();
							Info.CurrentFuncVariables.Add (new Info.FuncArgumentOrVars { Group = 0, ClassFunc = Info.CurrentFunc });
							Info.CurrentFuncVariables.Add (new Info.FuncArgumentOrVars { Group = 1, Vars = new Dictionary<string, AstStmt_DefVariable> () });
							ClassFuncs[j].BodyCodes.TraversalWraps ((_deep: 1, _group: 0, _loop: 0, _cb: ExprTraversals.Traversal));
							Info.CurrentFuncVariables = new List<Info.FuncArgumentOrVars> ();
							Info.CurrentFuncVariables.Add (new Info.FuncArgumentOrVars { Group = 0, ClassFunc = Info.CurrentFunc });
							Info.CurrentFuncVariables.Add (new Info.FuncArgumentOrVars { Group = 1, Vars = new Dictionary<string, AstStmt_DefVariable> () });
							ClassFuncs[j].BodyCodes.TraversalWraps ((_deep: 1, _group: 0, _loop: 1, _cb: ExprTraversals.Traversal));
							Info.CurrentFuncVariables = new List<Info.FuncArgumentOrVars> ();
							Info.CurrentFuncVariables.Add (new Info.FuncArgumentOrVars { Group = 0, ClassFunc = Info.CurrentFunc });
							Info.CurrentFuncVariables.Add (new Info.FuncArgumentOrVars { Group = 1, Vars = new Dictionary<string, AstStmt_DefVariable> () });
							ClassFuncs[j].BodyCodes.TraversalWraps ((_deep: 1, _group: 0, _loop: i, _cb: ExprTraversals.Traversal));
						}
					} else {
						ClassFuncs[j].BodyCodes.TraversalWraps ((_deep: 1, _group: 0, _loop: i, _cb: ExprTraversals.Traversal));
					}
				}
			}

			foreach (var _func in ClassFuncs)
				_func.ExpandFunc ();
			return true;
		}

		public override string GenerateCSharp (int _indent) {
			Info.CurrentClass = this;
			Info.CurrentFuncVariables = null;
			//
			var _sb = new StringBuilder ();
			_sb.AppendLine ($"{_indent.Indent ()}{Class.Level.ToString ().ToLower ()} class {CSharpFullName[(CSharpFullName.LastIndexOf ('.') + 1)..]} {{");
			foreach (var _var in ClassVars)
				_sb.Append (_var.GenerateCSharp (_indent + 1));
			foreach (var _func in ClassFuncs)
				_sb.Append (_func.GenerateCSharp (_indent + 1));
			_sb.AppendLine ($"{_indent.Indent ()}}}");
			return _sb.ToString ();
		}

		public int GetRealAttachVarPos (int _enum_index) {
			var _attach_type = ClassEnumItems[_enum_index].AttachType;
			if (_attach_type is AstType_Placeholder _ph_type)
				_attach_type = GetImplType (_ph_type.Name);
			for (int i = 1; i < ClassVars.Count; ++i) {
				if (_attach_type.IsSame (ClassVars[i].DataType))
					return i;
			}
			return -1;
		}

		public int GetTemplateNum () => 0;

		public IAstClass GetInst (List<IAstType> _templates, IToken _token = null) {
			if ((_templates?.Count ?? 0) > 0)
				throw new CodeException (_token, $"泛型类型无法再次指定模板参数");
			return this;
		}
	}
}