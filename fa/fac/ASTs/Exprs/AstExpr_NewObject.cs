﻿using fac.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fac.ASTs.Exprs {
	class AstExpr_NewObject: IAstExpr {
		public string DataType { get; set; }
		public List<(string _name, IAstExpr _value)> InitialValues { get; set; } = null; // 结构体初值，当有值时ConstructorArguments为空
		public List<IAstExpr> ConstructorArguments { get; set; } = null;                 // 构造函数参数，当有值时InitialValues为空



		public override void Traversal (int _deep, int _group, Func<IAstExpr, int, int, IAstExpr> _cb) {
			for (int i = 0; i < InitialValues.Count; ++i)
				InitialValues[i] = (_name: InitialValues[i]._name, _value: _cb (InitialValues[i]._value, _deep, _group));
			for (int i = 0; i < ConstructorArguments.Count; ++i)
				ConstructorArguments[i] = _cb (ConstructorArguments[i], _deep, _group);
		}

		public override IAstExpr TraversalCalcType (string _expect_type) {
			if (DataType == "") {
				if (_expect_type == "")
					throw new CodeException (Token, "无法猜测对象类型");
				DataType = _expect_type;
			}
			ExpectType = DataType;
			if (InitialValues != null) {
				var _classes = Info.GetClassFromName (DataType);
				if (_classes.Count == 0) {
					throw new CodeException (Token, $"未定义的标识符 {DataType}");
				} else if (_classes.Count == 1) {
					var _default_init_vals = new List<(string _name, IAstExpr _value)> ();
					foreach (var _vars in _classes[0].ClassVars) {
						int i = 0;
						for (; i < InitialValues.Count; ++i) {
							if (InitialValues[i]._name != _vars.Name)
								continue;
							InitialValues[i] = (_name: _vars.Name, _value: InitialValues[i]._value.TraversalCalcType (_vars.DataType.TypeStr));
							break;
						}
						if (i == InitialValues.Count) {
							if (_vars.DefaultValue == null)
								throw new CodeException (Token, $"未指定成员变量 {_vars.Name} 的初始值");
							_default_init_vals.Add ((_name: _vars.Name, _value: _vars.DefaultValue.TraversalCalcType (_vars.DataType.TypeStr)));
						}
					}
					InitialValues.AddRange (_default_init_vals);
				} else {
					throw new CodeException (Token, $"不明确的符号 {DataType}。可能为{string.Join ('、', from p in _classes select p.FullName)}");
				}
			} else {
				// TODO 检查构造函数参数
			}
			return AstExprTypeCast.Make (this, _expect_type);
		}

		public override string GuessType () => DataType;

		public override (string, string) GenerateCSharp (int _indent) {
			StringBuilder _psb = new StringBuilder (), _sb = new StringBuilder ();
			_sb.Append ($"new {DataType} ");
			if (InitialValues != null) {
				_sb.Append ($"{{ ");
				foreach (var _init in InitialValues) {
					var (_a, _b) = _init._value.GenerateCSharp (_indent);
					_psb.Append (_a);
					_sb.Append ($"{_init._name} = {_b}, ");
				}
				if (InitialValues.Any ())
					_sb.Remove (_sb.Length - 2, 2);
				_sb.Append ($" }}");
			} else {
				_sb.Append ($"(");
				foreach (var _arg in ConstructorArguments) {
					var (_a, _b) = _arg.GenerateCSharp (_indent);
					_psb.Append (_a);
					_sb.Append ($"{_b}, ");
				}
				if (ConstructorArguments.Any ())
					_sb.Remove (_sb.Length - 2, 2);
				_sb.Append ($")");
			}
			return (_psb.ToString (), _sb.ToString ());
		}

		public override bool AllowAssign () => false;
	}
}