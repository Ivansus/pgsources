using pg_web.Models;
using pg_web.sys.pg.hw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace pg_web.sys.pg {
	public class LabelModule : IModule {
		public const String SERVICE_NAME = "Labels";

		private pgworkDBEntities db;

		public void init() {
			db = new pgworkDBEntities();
		}


		public Label findOrCreateLabel(ushort _uLabelId)
		{
			try
			{
				return (
					from m in db.Labels
					where m.labelData == _uLabelId
					select m
				).First<Label>();
			}
			catch (Exception)
			{
				Label label = db.Labels.Create();
				label.labelData = _uLabelId;
				db.Labels.Add(label);
				db.SaveChanges();
				System.Diagnostics.Debug.Write("\n New Label, label data:" + _uLabelId);
				return label;
			}
		}
	}
}