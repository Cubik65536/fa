﻿using fac.ASTs.Exprs;
using fac.ASTs.Exprs.Names;
using fac.ASTs.Types;
using fac.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fac.ASTs.Stmts {
	public abstract class IAstStmt: IAstExpr {
		public static IAstStmt FromExpr (FaParser.ExprContext _ctx, bool _return) {
			if (_return) {
				return new AstStmt_Return { Token = _ctx?.Start ?? null, ReturnType = Info.CurrentFunc.ReturnType, Expr = FromContext (_ctx) };
			} else {
				return AstStmt_ExprWrap.MakeFromExpr (FromContext (_ctx));
			}
		}

		public static List<IAstStmt> FromStmt (FaParser.StmtContext _ctx) {
			var _stmts = new List<IAstStmt> ();
			if (_ctx == null) {
				return new List<IAstStmt> ();
			} else if (_ctx.ifStmt () != null) {
				return AstStmt_If.FromCtx (_ctx.ifStmt ());
			} else if (_ctx.whileStmt () != null) {
				var _whilestmt = new AstStmt_While { Token = _ctx.Start, IsDoWhile = false };
				_whilestmt.Condition = FromContext (_ctx.whileStmt ().expr ());
				_whilestmt.Contents = FromStmts (_ctx.whileStmt ().stmt ());
				_stmts.Add (_whilestmt);
			} else if (_ctx.whileStmt2 () != null) {
				var _whilestmt = new AstStmt_While { Token = _ctx.Start, IsDoWhile = true };
				_whilestmt.Condition = FromContext (_ctx.whileStmt2 ().expr ());
				_whilestmt.Contents = FromStmts (_ctx.whileStmt2 ().stmt ());
				_stmts.Add (_whilestmt);
			} else if (_ctx.forStmt () != null) {
				var _forstmt = new AstStmt_For { Token = _ctx.Start };
				_forstmt.Initializes = FromStmt (_ctx.forStmt ().stmt ()[0]);
				_forstmt.Condition = FromContext (_ctx.forStmt ().expr ()[0]);
				_forstmt.Increment = FromExprs (_ctx.forStmt ().expr ()[1..]);
				_forstmt.BodyCodes = FromStmts (_ctx.forStmt ().stmt ()[1..]);
				_stmts.Add (_forstmt);
			} else if (_ctx.forStmt2 () != null) {
				var _forstmt2 = new AstStmt_For2 { Token = _ctx.Start };
				_forstmt2.Iterator = new AstStmt_DefVariable { Token = _ctx.forStmt2 ().type ().Start, DataType = IAstType.FromContext (_ctx.forStmt2 ().type ()), VarName = _ctx.forStmt2 ().id ().GetText (), Expr = null };
				_forstmt2.ListContainer = FromContext (_ctx.forStmt2 ().expr ());
				_forstmt2.BodyCodes = FromStmts (_ctx.forStmt2 ().stmt ());
				_stmts.Add (_forstmt2);
			} else if (_ctx.quotStmtPart () != null) {
				_stmts.Add (new AstStmt_HuaQuotWrap { Token = _ctx.Start, Stmts = FromStmts (_ctx.quotStmtPart ().stmt ()) });
			} else if (_ctx.switchStmt2 () != null) {
				var _t = new AstStmt_Switch { Token = _ctx.Start, Condition = null };
				var _switch_items = _ctx.switchStmt2 ().switchStmtPart2 ();
				_t.CaseValues = null;
				_t.CaseWhen = (from p in _switch_items select FromContext (p.expr ())).ToList ();
				_t.CaseCodes = (from p in _switch_items select FromStmt (p.stmt ()).ToSingleStmt ()).ToList ();
				if (_ctx.switchStmt2 ().switchStmtPart2Last () != null) {
					_t.CaseWhen.Add (new AstExprName_Ignore { Token = _ctx.switchStmt2 ().switchStmtPart2Last ().Underline ().Symbol });
					_t.CaseCodes.Add (FromStmt (_ctx.switchStmt2 ().switchStmtPart2Last ().stmt ()).ToSingleStmt ());
				}
				_stmts.Add (_t);
			} else if (_ctx.switchStmt () != null) {
				var _t = new AstStmt_Switch { Token = _ctx.Start, Condition = FromContext (_ctx.switchStmt ().expr ()) };
				var _switch_items = _ctx.switchStmt ().switchStmtPart ();
				_t.CaseValues = (from p in _switch_items select FromContext (p.expr ()[0])).ToList ();
				_t.CaseWhen = (from p in _switch_items select p.expr ().Length > 1 ? FromContext (p.expr ()[1]) : null).ToList ();
				_t.CaseCodes = (from p in _switch_items select FromStmt (p.stmt ()).ToSingleStmt ()).ToList ();
				_stmts.Add (_t);
			} else if (_ctx.normalStmt () != null) {
				if (_ctx.normalStmt ().Continue () != null) {
					_stmts.Add (AstStmt_ExprWrap.MakeContinue (_ctx.Start));
				} else if (_ctx.normalStmt ().Break () != null) {
					_stmts.Add (AstStmt_ExprWrap.MakeBreak (_ctx.Start));
				} else if (_ctx.normalStmt ().Return () != null) {
					_stmts.Add (new AstStmt_Return { Token = _ctx.Start, ReturnType = Info.CurrentFunc.ReturnType, Expr = FromContext (_ctx.normalStmt ().expr ()) });
				} else {
					_stmts.Add (AstStmt_ExprWrap.MakeFromExpr (FromContext (_ctx.normalStmt ().expr ())));
				}
			} else if (_ctx.defVarStmt () != null) {
				return AstStmt_DefVariable.FromCtx (_ctx.defVarStmt ());
			} else {
				throw new UnimplException (_ctx.Start);
			}
			return _stmts;
		}

		public static List<IAstExpr> FromExprs (FaParser.ExprContext[] _ctxs) => (from p in _ctxs select FromContext (p)).ToList ();
		public static List<IAstStmt> FromStmts (FaParser.StmtContext[] _ctxs) => (from p in _ctxs select FromStmt (p)).CombileStmts ();
		public override IAstType GuessType () => throw new Exception ("不应执行此处代码");
		public override bool AllowAssign () => throw new Exception ("不应执行此处代码");
		public override (List<IAstStmt>, IAstExpr) ExpandExpr ((IAstExprName _var, AstStmt_Label _pos) _cache_err) => throw new Exception ("不应执行此处代码");

		/// <summary>
		/// 分解语句
		/// </summary>
		/// <param name="_cache_err">用于缓存错误的变量</param>
		/// <returns></returns>
		public abstract List<IAstStmt> ExpandStmt ((IAstExprName _var, AstStmt_Label _pos) _cache_err);

		/// <summary>
		/// 分解语句辅助类，此类在函数最外层辅助return空值；在if、switch等表达式中辅助内部语句向外赋空值
		/// </summary>
		/// <param name="_cache_err"></param>
		/// <param name="_callback"></param>
		/// <returns></returns>
		protected List<IAstStmt> ExpandStmtHelper ((IAstExprName _var, AstStmt_Label _pos) _cache_err, Func<Action<IAstExpr, IAstExpr>, List<IAstStmt>> _callback) => ExpandStmtHelper (_cache_err, false, _callback);
		protected List<IAstStmt> ExpandStmtHelper ((IAstExprName _var, AstStmt_Label _pos) _cache_err, bool _ignore_error, Func<Action<IAstExpr, IAstExpr>, List<IAstStmt>> _callback) {
			var _checks = new List<(IAstExpr, IAstExpr)> ();
			var _stmts = _callback ((_cond, _err) => {
				if (_cache_err == (null, null) && (!_ignore_error))
					throw new CodeException (Token, "函数返回值必须为可空才能自动返回错误");
				_checks.Add ((_cond, _err));
			});
			if (_ignore_error)
				return _stmts;
			//
			var _stmts2 = new List<IAstStmt> ();
			foreach (var (_cond, _err) in _checks) {
				_stmts2.Add (new AstStmt_If {
					Token = _cond.Token,
					Condition = _cond,
					IfTrueCodes = new List<IAstStmt> {
						AstStmt_ExprWrap.MakeAssign (_cache_err._var, AstExpr_AccessBuildIn.Optional_FromError (_cache_err._var.ExpectType, _err)),
						_cache_err._pos.GetRef (),
					},
				});
			}
			_stmts2.AddRange (_stmts);
			return _stmts2;
		}
	}
}
