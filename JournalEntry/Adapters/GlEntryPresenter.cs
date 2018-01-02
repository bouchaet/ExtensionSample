using System.Collections.Generic;
using Entities;
using JournalEntry.UseCases;

namespace JournalEntry.Adapters
{
    public class GlEntryPresenter : Port<IEnumerable<GlEntry>>, IPresenter<GlEntry>
    {
        private readonly ICollection<IView> _views;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ICsvSerializer _csvSerializer;

        protected override void PreTransfer(IEnumerable<GlEntry> data)
        {
            ShowAllElements(data);
        }

        protected override void PostTransfer(IEnumerable<GlEntry> data)
        {
        }

        protected override void PreReceive(IEnumerable<GlEntry> data)
        {
        }

        protected override void PostReceive(IEnumerable<GlEntry> data)
        {
        }

        public GlEntryPresenter(IJsonSerializer json, ICsvSerializer csv)
        {
            _views = new List<IView>();
            _jsonSerializer = json;
            _csvSerializer = csv;
        }

        public void AttachView(IView view)
        {
            _views.Add(view);
        }

        public void ShowAllElements(IEnumerable<GlEntry> entries)
        {
            foreach (var entry in entries)
            {
                foreach (var view in _views)
                {
                    view.RenderCsv(_csvSerializer.ToCsv(entry));
                    view.RenderJson(_jsonSerializer.ToJson(entry));
                }
            }
        }
    }
}