using System.Collections.Generic;

namespace EntityFrameworkIncludeSelfReference.Entities
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IList<Document> Documents { get; set; }
        public IList<Activity> ChildActivities { get; set; }
    }
}