﻿using System.Collections.Generic;
using System.Linq;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;
using Glimpse.Core.Tab.Assist;
using Glimpse.Orchard.Extensions;
using Glimpse.Orchard.Glimpse.Extensions;
using Glimpse.Orchard.Glimpse.Models;
using Glimpse.Orchard.Models.Messages;

namespace Glimpse.Orchard.Glimpse.Tabs.Widgets
{
    public class WidgetTab : TabBase, ITabSetup, IKey
    {

        public override object GetData(ITabContext context)
        {
            var messages = context.GetMessages<GlimpseMessage<WidgetMessage>>().ToList();

            if (!messages.Any())
            {
                return "There have been no Widget events recorded. If you think there should have been, check that the 'Glimpse for Orchard Widgets' feature is enabled.";
            }

            return messages;
        }

        public override string Name
        {
            get { return "Widgets"; }
        }

        public void Setup(ITabSetupContext context)
        {
            context.PersistMessages<GlimpseMessage<WidgetMessage>>();
        }

        public string Key
        {
            get { return "glimpse_orchard_widgets"; }
        }
    }

    public class WidgetMessagesConverter : SerializationConverter<IEnumerable<GlimpseMessage<WidgetMessage>>>
    {
        public override object Convert(IEnumerable<GlimpseMessage<WidgetMessage>> messages)
        {
            var root = new TabSection("Widget Title", "Widget Type", "Layer", "Layer Rule", "Zone", "Position", "Technical Name", "Build Display Duration");
            foreach (var message in messages.Unwrap().OrderByDescending(m => m.Duration))
            {
                root.AddRow()
                    .Column(message.Title)
                    .Column(message.Type)
                    .Column(message.Layer.Name)
                    .Column(message.Layer.LayerRule)
                    .Column(message.Zone)
                    .Column(message.Position)
                    .Column(message.TechnicalName)
                    .Column(message.Duration.ToTimingString());
            }

            root.AddRow()
                .Column("")
                .Column("")
                .Column("")
                .Column("")
                .Column("")
                .Column("")
                .Column("Total time:")
                .Column(messages.Unwrap().Sum(m => m.Duration.TotalMilliseconds).ToTimingString())
                .Selected();

            return root.Build();
        }
    }
}