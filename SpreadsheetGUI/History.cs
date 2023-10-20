using System;
namespace SpreadsheetGUI
{
	public class History
	{ 
		private Stack<(string, string, string)> backwards;
		private Stack<(string, string, string)> forwards;
        public History()
		{
			backwards = new();
			forwards = new();
		}

		public void addÇell(string address, string oldContents, string newContents)
		{
			backwards.Push((address, oldContents, newContents));
			forwards.Clear();
		}

		public bool canGoBack()
		{
			return backwards.Count > 0;
		}

        public bool canGoForward()
        {
            return forwards.Count > 0;
        }

		public (string, string) Back()
		{
			if (!canGoBack())
				throw new InvalidOperationException();
			var prev = backwards.Pop();
			forwards.Push(prev);
			return (prev.Item1, prev.Item2);
		}

        public (string, string) Forward()
        {
            if (!canGoForward())
                throw new InvalidOperationException();
            var prev = forwards.Pop();
            backwards.Push(prev);
            return (prev.Item1, prev.Item3);
        }
    }
}

