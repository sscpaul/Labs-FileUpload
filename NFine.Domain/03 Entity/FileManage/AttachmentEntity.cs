using System;

namespace NFine.Domain.Entity.FileManage
{
    public class AttachmentEntity : IEntity<AttachmentEntity>, ICreationAudited, IDeleteAudited
    {
        public string F_Id { get; set; }
        public string F_Name { get; set; }
        public string F_ExtName { get; set; }
        public string F_Path { get; set; }
        public int F_Size { get; set; }
        public bool? F_DeleteMark { get; set; }
        public string F_Description { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public string F_CreatorUserId { get; set; }
        public DateTime? F_DeleteTime { get; set; }
        public string F_DeleteUserId { get; set; }
    }
}
