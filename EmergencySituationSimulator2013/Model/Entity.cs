﻿using System;
using System.Collections.Generic;
using EmergencySituationSimulator2013.Visitors;

namespace EmergencySituationSimulator2013.Model
{
    public abstract class Entity
    {
        public static List<Entity> Instances = new List<Entity>();
        
        private readonly string _id;

        public string Id { get { return _id; } }

        public Location Location { get; set; }

        public string Name;

        protected Entity()
        {
            //Guid = Guid.NewGuid();

            // Base64url
            /*_id = Convert.ToBase64String(Guid.ToByteArray())
                .Substring(0,10).Replace('+', '-').Replace('/', '_');*/

            // Ascii85 <3
            //_id = Logos.Utility.Ascii85.Encode(Guid.ToByteArray()).Substring(0, 10);
            _id = Oracle.GenerateId();

            Location = new Location();

            Instances.Add(this);
        }

        public virtual void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public static void VisitAll(IVisitor visitor)
        {
            foreach (var instance in Instances)
            {
                instance.Accept(visitor);
            }
        }


    }
}
