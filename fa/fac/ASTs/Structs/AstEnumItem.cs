﻿using fac.AntlrTools;
using fac.ASTs.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fac.ASTs.Structs {
	public class AstEnumItem: IAst {
		public string Name { init; get; }
		public IAstType AttachType { init; get; } = null;



		public AstEnumItem (FaParser.ClassEnumItemContext _ctx) {
			Token = _ctx.Start;
			//
			Name = _ctx.id ().GetText ();
			//
			if (_ctx.type () != null)
				AttachType = IAstType.FromContext (_ctx.type ());
		}

		public override string GenerateCSharp (int _indent) => throw new NotImplementedException ();
	}
}