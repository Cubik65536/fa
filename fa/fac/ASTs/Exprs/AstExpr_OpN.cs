﻿using fac.AntlrTools;
using fac.ASTs.Exprs.Names;
using fac.ASTs.Stmts;
using fac.ASTs.Types;
using fac.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fac.ASTs.Exprs {
	public class AstExpr_OpN: IAstExpr {
		public IAstExpr Value { get; set; }
		public List<IAstExpr> Arguments { get; set; }



		private bool CheckArguments (AstType_Func _functype) {
			if (_functype.ArgumentTypes.Count > 0 && _functype.ArgumentTypes[^1] is AstType_ArrayWrap _arrtype && _arrtype.Params) {
				if (Arguments.Count < _functype.ArgumentTypes.Count - 1)
					throw new CodeException (Token, "函数调用传入的参数数量不匹配");
				for (int i = 0; i < _functype.ArgumentTypes.Count - 1; ++i)
#warning 移除所有的   .TraversalCalcType (
					Arguments[i] = Arguments[i].TraversalCalcType (_functype.ArgumentTypes[i]);
				if (_functype.ArgumentTypes.Count == Arguments.Count) {
					var _type1 = Arguments[^1].GuessType ();
					if (_type1 == null) {
						return false;
					} else if (_type1.IsSame (_arrtype)) {
						try {
							var _arg = Arguments[^1].TraversalCalcType (_functype.ArgumentTypes[^1]);
							if (_arg == null)
								return false;
							Arguments[^1] = _arg;
							return true;
						} catch (Exception) {
						}
					}
				}
				for (int i = _functype.ArgumentTypes.Count - 1; i < Arguments.Count; ++i) {
					var _type1 = Arguments[i].TraversalCalcType (_arrtype.ItemType);
					if (_type1 == null) {
						return false;
					} else {
						Arguments[i] = _type1;
					}
				}
			} else {
				if (_functype.ArgumentTypes.Count != Arguments.Count)
					throw new CodeException (Token, "函数调用传入的参数数量不匹配");
				for (int i = 0; i < _functype.ArgumentTypes.Count; ++i) {
					var _type1 = Arguments[i].TraversalCalcType (_functype.ArgumentTypes[i]);
					if (_type1 == null) {
						return false;
					} else {
						Arguments[i] = _type1;
					}
				}
			}
			return true;
		}

		public override void Traversal ((int _deep, int _group, int _loop, Func<IAstExpr, int, int, int, IAstExpr> _cb) _trav) {
			Value = Value.TraversalWrap (_trav);
			Arguments.TraversalWraps (_trav);
		}

		public override IAstExpr TraversalCalcType (IAstType _expect_type) {
			// 单独处理枚举类型，另一部分代码位于AstExpr_BaseId.cs
			//if (_expect_type is AstType_Class _class_type && (_class_type.Class.ClassEnumItems?.Count ?? 0) > 0
			//	&& Value is AstExpr_BaseId _bi_expr
			//	&& Arguments.Count == 1) {
			//	if (Arguments[0] is AstExpr_BaseId _bi_expr1) {
			//		var _class_enum = AstExprName_ClassEnum_New.FindFromName (Token, _class_type.Class, _bi_expr.Id);
			//		var _is_expr = AstExpr_Is.FromContext (Token, null, _class_enum, _bi_expr1.Id);
			//		_is_expr.ExpectType = _expect_type;
			//		return _is_expr;
			//	} else {
			//		var _ce_expr = AstExprName_ClassEnum_New.FindFromNameUncheckAttach (Token, _class_type.Class, _bi_expr.Id);
			//		_ce_expr.AttachExpr = Arguments[0];
			//		_ce_expr.ExpectType = _expect_type;
			//		return _ce_expr;
			//	}
			//}
			//
			Value = Value.TraversalCalcType (null);
			if (Value is AstExprName_ClassFunc _funcexpr) {
				if (_funcexpr.ThisObject != null)
					Arguments.Insert (0, _funcexpr.ThisObject);
				if (!CheckArguments (_funcexpr.Class.ClassFuncs[_funcexpr.FunctionIndex].FuncType))
					return null;
				if (_funcexpr.ThisObject != null)
					Arguments.RemoveAt (0);
				ExpectType = _funcexpr.Class.ClassFuncs[_funcexpr.FunctionIndex].ReturnType;
				return AstExprTypeCast.Make (this, _expect_type);
			} else {
				if (Value is AstExprName_BuildIn _biexpr)
					Value.GuessType ();
				if (Value.ExpectType is AstType_Func _functype) {
					if (!CheckArguments (_functype))
						return null;
					ExpectType = _functype.ReturnType;
					return AstExprTypeCast.Make (this, _expect_type);
				} else {
					throw new CodeException (Token, "表达式无法当做方法进行调用");
				}
			}
		}

		public override IAstType GuessType () {
			if (Value is AstExprName_ClassFunc _funcexpr) {
				return _funcexpr.Class.ClassFuncs[_funcexpr.FunctionIndex].ReturnType;
			} else {
				return (Value.GuessType () as AstType_Func).ReturnType;
			}
		}

		public override (List<IAstStmt>, IAstExpr) ExpandExpr ((IAstExprName _var, AstStmt_Label _pos)? _cache_err) {
			var (_stmts, _val) = Value.ExpandExpr (_cache_err);
			Value = _val;
			for (int i = 0; i < Arguments.Count; ++i) {
				var (_stmts1, _val1) = Arguments[i].ExpandExpr (_cache_err);
				_stmts.AddRange (_stmts1);
				Arguments[i] = _val1;
			}

			var _arg_types = (Value.ExpectType as AstType_Func).ArgumentTypes;
			//if (Value is AstExprName_ClassFunc _funcexpr && _funcexpr.ThisObject != null)
			//	_arg_types = _arg_types.Skip (1).ToList ();
			// 如果最后一个参数为params列表
			if (_arg_types.Count > 0 && _arg_types[^1] is AstType_ArrayWrap _awrap && _awrap.Params && Value is not AstExprName_BuildIn) {
				bool _process_last = _arg_types.Count == Arguments.Count && Arguments[^1].ExpectType.IsSame (_awrap);
				if (!_process_last) {
					// 用户未处理
					(_stmts, _val) = new AstExpr_Array { Token = Arguments[_arg_types.Count - 1].Token, ItemDataType = _awrap.ItemType, InitValues = Arguments.Skip (_arg_types.Count - 1).ToList (), ExpectType = _awrap }.ExpandExpr (_cache_err);
					Arguments.RemoveRange (_arg_types.Count - 1, Arguments.Count - (_arg_types.Count - 1));
					_stmts.AddRange (_stmts);
					Arguments.Add (_val);
				}
			}
			return (_stmts, this);
		}

		public override string GenerateCSharp (int _indent) {
			var _sb = new StringBuilder ();
			var _arg_types = (Value.ExpectType as AstType_Func).ArgumentTypes;
			//if (Value is AstExprName_ClassFunc _funcexpr && _funcexpr.ThisObject != null)
			//	_arg_types = _arg_types.Skip (1).ToList ();
			var _b = Value.GenerateCSharp (_indent);
			_sb.Append ($"{_b} (");
			for (int i = 0; i < Arguments.Count; ++i) {
				_b = Arguments[i].GenerateCSharp (_indent);
				if (Value is not AstExprName_BuildIn) {
					if (_arg_types[i].Mut)
						_sb.Append ($"ref ");
				}
				_sb.Append ($"{_b}, ");
			}
			if (Value is AstExprName_BuildIn _biexpr && _biexpr.Name.EndsWith ("AllText"))
				_sb.Append ($"Encoding.UTF8, ");
			if (Arguments.Any ())
				_sb.Remove (_sb.Length - 2, 2);
			_sb.Append (")");
			if (Value is AstExprName_BuildIn _biexpr1 && _biexpr1.Name == "Directory.GetFiles")
				_sb.Append (".ToList ()");
			return _sb.ToString ();
		}

		public override bool AllowAssign () => false;
	}
}
