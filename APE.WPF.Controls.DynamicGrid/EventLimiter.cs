// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventLimiter.cs" company="APE-Engineering GmbH">
//  This source code is provided under the CodeProject OpenLicence V1.0 
// </copyright>
// <summary>
//   Implementation that limits the execution of an action on a given dispatcher.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace APE.WPF.Controls.DynamicGrid
{
	using System;
	using System.Windows.Threading;

	/// <summary>
	/// Implementation that limits the execution of an action on a given dispatcher.
	/// </summary>
	/// <typeparam name="T">Type of the parameter of the executing action</typeparam>
	public class EventLimiter<T>
	{
		/// <summary>
		/// Dispatcher, on which the action is dispatched.
		/// </summary>
		private readonly Dispatcher dispatcher;

		/// <summary>
		/// Priority the action is executed with.
		/// </summary>
		private readonly DispatcherPriority dispatcherPriority;

		/// <summary>
		/// Action that is executed.
		/// </summary>
		private readonly Action<T> executeAction;

		/// <summary>
		/// Timer that is used to limit the events.
		/// </summary>
		private readonly DispatcherTimer timer;

		/// <summary>
		/// Parameter for the next invocation.
		/// </summary>
		private T nextEventParameter;

		/// <summary>
		/// Value whether the limiting is currently active.
		/// </summary>
		private bool isLimiting;
		
		/// <summary>
		/// Value that - when the limiting is active - indicates if an invocation is needed after limiting is finished.
		/// </summary>
		private bool invocationRequired;

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <param name="dispatcher">Dispatcher, on which the action is dispatched.</param>
		/// <param name="priority">Priority, the action is executed with.</param>
		/// <param name="checkInterval">Time between invocations (in milliseconds)</param>
		/// <param name="executeAction">Action that is executed.</param>
		public EventLimiter(Dispatcher dispatcher, DispatcherPriority priority, int checkInterval, Action<T> executeAction)
		{
			if (dispatcher == null) throw new ArgumentNullException("dispatcher");
			if (executeAction == null) throw new ArgumentNullException("executeAction");
			
			this.dispatcher = dispatcher;
			this.dispatcherPriority = priority;
			this.executeAction = executeAction;

			this.timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, checkInterval), priority, this.TimerTickHandler, dispatcher);
		}

		/// <summary>
		/// Trigger the invocation of the action.
		/// </summary>
		/// <param name="arg">Argument that is passed to the action.</param>
		public void Trigger(T arg)
		{
			if (this.isLimiting)
			{
				this.nextEventParameter = arg;
				this.invocationRequired = true;
			}
			else
			{
				this.dispatcher.BeginInvoke(this.dispatcherPriority, this.executeAction, arg);
				this.isLimiting = true;
				this.timer.Start();
			}
		}

		/// <summary>
		/// This method is invoked when the limiting should be disabled and the action may be executed again.
		/// </summary>
		private void TimerTickHandler(object sender, EventArgs eventArgs)
		{
			this.timer.Stop();

			if (this.invocationRequired)
				this.dispatcher.BeginInvoke(this.dispatcherPriority, this.executeAction, this.nextEventParameter);

			this.isLimiting = false;
		}
	}
}
