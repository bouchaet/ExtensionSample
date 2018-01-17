using System.Collections.Generic;
using Entities;
using JournalEntry.UseCases;

namespace JournalEntry.Adapters
{
    public class GlEntryPresenter : Port<IEnumerable<PartnerGlEntry>>, IPresenter<PartnerGlEntry>
    {
        private readonly ICollection<IView> _views;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ICsvSerializer _csvSerializer;

        protected override void PreTransfer(IEnumerable<PartnerGlEntry> data)
        {
            ShowAllElements(data);
        }

        protected override void PostTransfer(IEnumerable<PartnerGlEntry> data)
        {
        }

        protected override void PreReceive(IEnumerable<PartnerGlEntry> data)
        {
        }

        protected override void PostReceive(IEnumerable<PartnerGlEntry> data)
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

        public void ShowAllElements(IEnumerable<PartnerGlEntry> entries)
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