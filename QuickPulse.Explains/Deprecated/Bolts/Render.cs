using QuickPulse.Bolts;
using QuickPulse.Explains.Deprecated;

namespace QuickPulse.Explains.Deprecated.Bolts;

public static class Render
{
    public static readonly Flow<DocAttribute> AsMarkdown =
        from doc in Pulse.Start<DocAttribute>()
        let headingLevel = doc.Order.Split('-').Length
        from rcaption in Pulse
            .NoOp(/* ---------------- Render Caption  ---------------- */ )
        let caption = doc.Caption
        let hasCaption = !string.IsNullOrEmpty(doc.Caption)
        let headingMarker = new string('#', headingLevel)
        let captionLine = $"{headingMarker} {caption}"
        from _t2 in Pulse.TraceIf(hasCaption, () => captionLine)
        from rcontent in Pulse
            .NoOp(/* ---------------- Render content  ---------------- */ )
        let content = doc.Content
        let hasContent = !string.IsNullOrEmpty(content)
        from _t3 in Pulse.TraceIf(hasContent, () => content)
        from end in Pulse
            .NoOp(/* ---------------- End of content  ---------------- */ )
        select doc;
}
