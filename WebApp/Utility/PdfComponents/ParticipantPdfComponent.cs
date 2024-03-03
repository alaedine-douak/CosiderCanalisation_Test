using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace WebApp.Utility.PdfComponents;

public class ParticipantPdfComponent : IComponent
{
    private string ParticipantJobe { get; }

    private string ParticipantName { get; }

    public ParticipantPdfComponent(string participantJobe, string participantName)
    {
        ParticipantJobe = participantJobe;
        ParticipantName = participantName;
    }

    public void Compose(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(2);

            column.Item().BorderBottom(1).PaddingBottom(2).Text(ParticipantJobe).Italic().FontSize(11);
            column.Item().Text(ParticipantName).ExtraBold().FontSize(12);
            column.Item().Text($"INFORMATION DE {ParticipantJobe}").Italic().FontSize(11);
        });
    }
}
