using NFine.Domain.Entity.FileManage;
using System.Data.Entity.ModelConfiguration;

namespace NFine.Mapping.FileManage
{
    public class AttachmentMap : EntityTypeConfiguration<AttachmentEntity>
    {
        public AttachmentMap()
        {
            this.ToTable("T_Attachment");
            this.HasKey(t => t.F_Id);
        }
    }
}
