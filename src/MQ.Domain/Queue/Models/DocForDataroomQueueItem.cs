using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MQ.Domain.Queue.Models;

public class DocForDataroomQueueItem
{
    public int TriesCount { get; set; }

    public string ContentPath { get; set; } = default!;

    public string DocNum { get; set; } = default!;
    public DateTime DocDate { get; set; }
    public string DocType { get; set; } = default!;
    public string CompanyEgrp { get; set; } = default!;
    public string ContragentEgrp { get; set; } = default!;
    public string ContragentName { get; set; } = default!;
    public string? MainDocNum { get; set; }
    public DateTime? MainDocDate { get; set; }
    public Guid? BusId { get; set; }
    public string? LinkToAgreement { get; set; }
    public string? ProjectCode { get; set; }
    public int? RequestId { get; set; }

    public string FileName => $"{DocType}_{DocNum}_{DocDate:dd-MM-yyyy}.pdf";


    public static DocForDataroomQueueItem? ParseQueueItem(byte[] existingItem)
    {
        if (existingItem == null || existingItem.Length == 0)
        {
            throw new ArgumentNullException(nameof(existingItem));
        }

        var jsonData = Encoding.UTF8.GetString(existingItem);
        return JsonSerializer.Deserialize<DocForDataroomQueueItem>(jsonData);
    }

    public byte[] ToByteArray()
    {
        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(this));
    }
}
