using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using MWC.BL;
using MWC.iOS.Screens.Common.Session;

namespace MWC.iOS.Screens.iPhone.Sessions
{
	/// <summary>
	/// Speakers screen. Derives from MonoTouch.Dialog's DialogViewController to do 
	/// the heavy lifting for table population.
	/// </summary>
	public partial class SessionsScreen : DialogViewController
	{
		protected SessionDetailsScreen _sessionDetailsScreen;

		public SessionsScreen () : base (UITableViewStyle.Grouped, null)
		{
			if(BL.Managers.UpdateManager.IsUpdating)
			{
				Console.WriteLine("Waiting for updates to finish (sessions screen)");
				BL.Managers.UpdateManager.UpdateFinished += (sender, e) => {
					Console.WriteLine("Updates finished, going to populate sessions screen.");
					this.InvokeOnMainThread ( () => { this.PopulatePage(); } );
					//TODO: unsubscribe from static event so GC can clean
				};
			}
			else
			{
				Console.WriteLine("not updating, populating sessions.");
				this.PopulatePage();
			}
		}
		
		/// <summary>
		/// Populates the page with sessions, grouped by time slot
		/// </summary>
		public void PopulatePage()
		{
			// get the sessions from the database
			var sessions = BL.Managers.SessionManager.GetSessions ();
			
			Root = 	new RootElement ("Sessions") {
					from session in sessions
						group session by session.Start.Ticks into timeslot
						orderby timeslot.Key
						select new Section (new DateTime (timeslot.Key).ToString("dddd HH:mm") ) {
						from eachSession in timeslot
						   select (Element) new MWC.iOS.UI.CustomElements.SessionElement (eachSession)
			}};

		}		
	}
}