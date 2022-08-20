using MQ.DataroomImportApi.Exceptions;
using MQ.Domain.Queue.Models;
using System.ComponentModel.DataAnnotations;

namespace MQ.DataroomImportApi.Models
{
    public class ImportRequestModel
    {
        [Required]
        public Guid IdempotencyKey { get; set; }

        [Required]
        public IFormFile Content { get; set; }

        [Required]
        public string DocType { get; set; }

        [Required]
        public string DocNum { get; set; }

        [Required]
        public DateTime DocDate { get; set; }

        [Required]
        public string KernelEgrp { get; set; }

        [Required]
        public string ContragentEgrp { get; set; }

        [Required]
        public string ContragentName { get; set; }

        public string? MainDocNum { get; set; }
        public DateTime? MainDocDate { get; set; }
        public string? LinkToAgreement { get; set; }
        public Guid? BusId { get; set; }
        public string? ProjectCode { get; set; }
        public int? RequestId { get; set; }

        //В будущем поддержку файлов в formdata должны добавить, но пока парсим вручную
        public ImportRequestModel(HttpContext ctx)
        {
            if (ctx.Request.Form.Files.Count == 0)
                throw new ApiException(ErrorCodes.ArgumentIsNull, 500, $"File is missing");
            if (string.IsNullOrEmpty(ctx.Request.Form[nameof(IdempotencyKey)]))
                throw new ApiException(ErrorCodes.ArgumentIsNull, 500, $"{nameof(IdempotencyKey)} is null");
            if (string.IsNullOrEmpty(ctx.Request.Form[nameof(DocDate)]))
                throw new ApiException(ErrorCodes.ArgumentIsNull, 500, $"{nameof(DocDate)} is null");
            if (string.IsNullOrEmpty(ctx.Request.Form[nameof(DocNum)]))
                throw new ApiException(ErrorCodes.ArgumentIsNull, 500, $"{nameof(DocNum)} is null");
            if (string.IsNullOrEmpty(ctx.Request.Form[nameof(DocType)]))
                throw new ApiException(ErrorCodes.ArgumentIsNull, 500, $"{nameof(DocType)} is null");
            if (string.IsNullOrEmpty(ctx.Request.Form[nameof(KernelEgrp)]))
                throw new ApiException(ErrorCodes.ArgumentIsNull, 500, $"{nameof(KernelEgrp)} is null");
            if (string.IsNullOrEmpty(ctx.Request.Form[nameof(ContragentEgrp)]))
                throw new ApiException(ErrorCodes.ArgumentIsNull, 500, $"{nameof(ContragentEgrp)} is null");

            var file = ctx.Request.Form.Files[0];

            DocDate = DateTime.Parse(ctx.Request.Form[nameof(DocDate)]);
            DocType = ctx.Request.Form[nameof(DocType)];
            DocNum = ctx.Request.Form[nameof(DocNum)];
            Content = file;
            MainDocDate = string.IsNullOrEmpty(ctx.Request.Form[nameof(MainDocDate)]) ? null : DateTime.Parse(ctx.Request.Form[nameof(MainDocDate)]);
            MainDocNum = ctx.Request.Form[nameof(MainDocNum)];
            KernelEgrp = ctx.Request.Form[nameof(KernelEgrp)];
            ContragentEgrp = ctx.Request.Form[nameof(ContragentEgrp)];
            ContragentName = ctx.Request.Form[nameof(ContragentName)];
            IdempotencyKey = new Guid(ctx.Request.Form[nameof(IdempotencyKey)]);
            LinkToAgreement = ctx.Request.Form[nameof(LinkToAgreement)];
            ProjectCode = ctx.Request.Form[nameof(ProjectCode)];

            BusId = string.IsNullOrEmpty(ctx.Request.Form[nameof(BusId)]) ?
                null : new Guid(ctx.Request.Form[nameof(BusId)]);

            RequestId = string.IsNullOrEmpty(ctx.Request.Form[nameof(RequestId)]) ?
                null : int.Parse(ctx.Request.Form[nameof(RequestId)]);
        }

        public DocForDataroomQueueItem ToQueueItem(string filePath)
        {
            DocForDataroomQueueItem item = new()
            {
                DocNum = DocNum,
                MainDocNum = MainDocNum,
                DocDate = DocDate,
                MainDocDate = MainDocDate,
                DocType = DocType,
                CompanyEgrp = KernelEgrp,
                ContragentEgrp = ContragentEgrp, 
                ContragentName = ContragentName, 
                BusId = BusId, 
                LinkToAgreement = LinkToAgreement, 
                ContentPath = filePath, 
                ProjectCode = ProjectCode, 
                RequestId = RequestId
            };

            return item;
        }
    }
}
