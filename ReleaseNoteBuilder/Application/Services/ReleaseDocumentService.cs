using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReleaseNoteBuilder.Application.DTOs;
using ReleaseNoteBuilder.Application.Interfaces;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ReleaseNoteBuilder.Application.Services;

/// <summary>
/// Service to generate Word documents for release notes
/// </summary>
public class ReleaseDocumentService : IReleaseDocumentService
{
    public byte[] GenerateReleaseDocument(
        string projectName,
        string environment,
        BuildDto projectBuild,
        BuildDto configBuild,
        List<WorkItemDto> bugs,
        List<WorkItemDto> pbis,
        string requestedBy = "Admin User",
        string approvedBy = "Release Management")
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
            {
                var mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                var body = mainPart.Document.AppendChild(new Body());

                // Title
                AddHeading(body, "RELEASE NOTES", 1);
                AddParagraph(body, "");

                // Header Information
                AddHeading(body, "Release Information", 2);
                AddTableRow(body, "Requested By:", requestedBy);
                AddTableRow(body, "Date:", DateTime.Now.ToString("yyyy-MM-dd"));
                AddTableRow(body, "Deployment Target Date:", DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
                AddParagraph(body, "");

                // Build Version Numbers
                AddHeading(body, "Build Version Numbers", 2);
                AddTableRow(body, "Project:", projectName);
                AddTableRow(body, "Release Build Version:", projectBuild?.Version ?? "N/A");
                AddTableRow(body, $"Config_{environment}:", configBuild?.Version ?? "N/A");
                AddTableRow(body, "Destination Environment:", environment);
                AddParagraph(body, "");

                // User Stories Section
                if (pbis.Any())
                {
                    AddHeading(body, "User Stories", 2);
                    AddWorkItemsTable(body, pbis, "User Story");
                    AddParagraph(body, "");
                }

                // Defects Section
                if (bugs.Any())
                {
                    AddHeading(body, "Defects to be Deployed", 2);
                    AddWorkItemsTable(body, bugs, "Defect");
                    AddParagraph(body, "");
                }

                // Summary of Changes
                AddHeading(body, "Summary of Changes", 2);
                var totalChanges = bugs.Count + pbis.Count;
                AddParagraph(body, $"Total Items: {totalChanges}");
                AddParagraph(body, $"  • Defects: {bugs.Count}");
                AddParagraph(body, $"  • User Stories: {pbis.Count}");
                AddParagraph(body, "");

                // Approval Section
                AddHeading(body, "Approvals", 2);
                AddTableRow(body, "Approved By:", approvedBy);
                AddTableRow(body, "Operational Team:", "Pending");
                AddTableRow(body, "Release Management:", "Pending");

                mainPart.Document.Save();
            }

            return memoryStream.ToArray();
        }
    }

    private void AddHeading(Body body, string text, int level)
    {
        var paragraph = body.AppendChild(new Paragraph());
        var pPr = paragraph.AppendChild(new ParagraphProperties());

        var pStyle = new ParagraphStyleId { Val = level == 1 ? "Heading1" : "Heading2" };
        pPr.AppendChild(pStyle);

        var run = paragraph.AppendChild(new Run());
        var rPr = run.AppendChild(new RunProperties());

        if (level == 1)
        {
            rPr.AppendChild(new Bold());
            rPr.AppendChild(new FontSize { Val = "28" });
        }
        else
        {
            rPr.AppendChild(new Bold());
            rPr.AppendChild(new FontSize { Val = "24" });
        }

        run.AppendChild(new Text { Text = text });
    }

    private void AddParagraph(Body body, string text)
    {
        var paragraph = body.AppendChild(new Paragraph());
        if (!string.IsNullOrEmpty(text))
        {
            var run = paragraph.AppendChild(new Run());
            run.AppendChild(new Text { Text = text });
        }
    }

    private void AddTableRow(Body body, string label, string value)
    {
        var table = body.Elements<Table>().FirstOrDefault();
        if (table == null)
        {
            table = body.AppendChild(new Table());
            var tbl = table.AppendChild(new TableProperties());
            tbl.AppendChild(new TableBorders
            {
                TopBorder = new TopBorder { Val = BorderValues.Single, Size = 12 },
                BottomBorder = new BottomBorder { Val = BorderValues.Single, Size = 12 },
                LeftBorder = new LeftBorder { Val = BorderValues.Single, Size = 12 },
                RightBorder = new RightBorder { Val = BorderValues.Single, Size = 12 },
                InsideHorizontalBorder = new InsideHorizontalBorder { Val = BorderValues.Single, Size = 12 },
                InsideVerticalBorder = new InsideVerticalBorder { Val = BorderValues.Single, Size = 12 }
            });
        }

        var row = table.AppendChild(new TableRow());

        // Label Cell
        var labelCell = row.AppendChild(new TableCell());
        var labelPara = labelCell.AppendChild(new Paragraph());
        var labelRun = labelPara.AppendChild(new Run());
        var labelRPr = labelRun.AppendChild(new RunProperties());
        labelRPr.AppendChild(new Bold());
        labelRun.AppendChild(new Text { Text = label });

        // Value Cell
        var valueCell = row.AppendChild(new TableCell());
        var valuePara = valueCell.AppendChild(new Paragraph());
        var valueRun = valuePara.AppendChild(new Run());
        valueRun.AppendChild(new Text { Text = value });
    }

    private void AddWorkItemsTable(Body body, List<WorkItemDto> items, string itemType)
    {
        var table = body.AppendChild(new Table());
        var tbl = table.AppendChild(new TableProperties());
        tbl.AppendChild(new TableBorders
        {
            TopBorder = new TopBorder { Val = BorderValues.Single, Size = 12 },
            BottomBorder = new BottomBorder { Val = BorderValues.Single, Size = 12 },
            LeftBorder = new LeftBorder { Val = BorderValues.Single, Size = 12 },
            RightBorder = new RightBorder { Val = BorderValues.Single, Size = 12 },
            InsideHorizontalBorder = new InsideHorizontalBorder { Val = BorderValues.Single, Size = 12 },
            InsideVerticalBorder = new InsideVerticalBorder { Val = BorderValues.Single, Size = 12 }
        });

        // Header Row
        var headerRow = table.AppendChild(new TableRow());
        AddTableHeaderCell(headerRow, "ID");
        AddTableHeaderCell(headerRow, "Title");
        AddTableHeaderCell(headerRow, "Description");
        AddTableHeaderCell(headerRow, "Author");
        AddTableHeaderCell(headerRow, "Priority");
        AddTableHeaderCell(headerRow, "Status");

        // Data Rows
        foreach (var item in items)
        {
            var row = table.AppendChild(new TableRow());
            AddTableDataCell(row, item.AdoWorkItemId);
            AddTableDataCell(row, item.Title);
            AddTableDataCell(row, item.Description);
            AddTableDataCell(row, item.Author);
            AddTableDataCell(row, item.Priority);
            AddTableDataCell(row, item.Status);
        }
    }

    private void AddTableHeaderCell(TableRow row, string text)
    {
        var cell = row.AppendChild(new TableCell());
        var para = cell.AppendChild(new Paragraph());
        var run = para.AppendChild(new Run());
        var rPr = run.AppendChild(new RunProperties());
        rPr.AppendChild(new Bold());
        rPr.AppendChild(new Shading { Fill = "D3D3D3" });
        run.AppendChild(new Text { Text = text });
    }

    private void AddTableDataCell(TableRow row, string text)
    {
        var cell = row.AppendChild(new TableCell());
        var para = cell.AppendChild(new Paragraph());
        var run = para.AppendChild(new Run());
        run.AppendChild(new Text { Text = text ?? "" });
    }
}