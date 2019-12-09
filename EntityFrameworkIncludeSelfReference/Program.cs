using EntityFrameworkIncludeSelfReference.Contexts;
using EntityFrameworkIncludeSelfReference.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace EntityFrameworkIncludeSelfReference
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (SelfReferenceContext context = new SelfReferenceContext())
            {
                var act1 = new Activity { Name = "Act1", Documents = new List<Document>(), ChildActivities = new List<Activity>() };
                var act2 = new Activity { Name = "Act2", Documents = new List<Document>(), ChildActivities = new List<Activity>() };
                var act3 = new Activity { Name = "Act3", Documents = new List<Document>(), ChildActivities = new List<Activity>() };
                var act4 = new Activity { Name = "Act4", Documents = new List<Document>(), ChildActivities = new List<Activity>() };
                var act5 = new Activity { Name = "Act5", Documents = new List<Document>(), ChildActivities = new List<Activity>() };
                var act6 = new Activity { Name = "Act6", Documents = new List<Document>(), ChildActivities = new List<Activity>() };
                var act7 = new Activity { Name = "Act7", Documents = new List<Document>(), ChildActivities = new List<Activity>() };
                var act8 = new Activity { Name = "Act8", Documents = new List<Document>(), ChildActivities = new List<Activity>() };
                var act9 = new Activity { Name = "Act9", Documents = new List<Document>(), ChildActivities = new List<Activity>() };

                var doc1 = new Document { Name = "Doc1" };
                var doc2 = new Document { Name = "Doc2" };
                var doc3 = new Document { Name = "Doc3" };
                var doc4 = new Document { Name = "Doc4" };
                var doc5 = new Document { Name = "Doc5" };
                var doc6 = new Document { Name = "Doc6" };
                var doc7 = new Document { Name = "Doc7" };
                var doc8 = new Document { Name = "Doc8" };

                act1.Documents.Add(doc1);
                act1.ChildActivities.Add(act2);

                act2.Documents.Add(doc2);
                act2.ChildActivities.Add(act3);

                act3.Documents.Add(doc3);
                act3.ChildActivities.Add(act4);

                act4.Documents.Add(doc4);
                act4.ChildActivities.Add(act5);

                act5.Documents.Add(doc5);
                act5.ChildActivities.Add(act6);

                act6.Documents.Add(doc6);
                act6.ChildActivities.Add(act7);

                act7.Documents.Add(doc7);
                act7.ChildActivities.Add(act8);

                act8.Documents.Add(doc8);
                act8.ChildActivities.Add(act9);

                context.Activities.Add(act1);
                context.SaveChanges();

                var activities = context.Activities
                    .Include(a => a.Documents)
                    .Include(a => a.ChildActivities.Select(ca => ca.Documents))
                    .ToList();
            }
        }
    }
}