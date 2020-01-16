using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Retrospective_Core.Models {
    public class RetroColumn {

        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public ICollection<RetroCard> RetroCards { get; set; } = new List<RetroCard>();

        public ICollection<RetroFamily> RetroFamilies { get; set; } = new List<RetroFamily>();

        [ForeignKey("RetrospectiveId")]
        public int RetrospectiveId { get; set; }

        public virtual Retrospective Retrospective { get; set; }
    }
}
