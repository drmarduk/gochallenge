using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace gochallenge
{
    public class Sample
    {
        private CultureInfo _c;
        public string File { get; set; }
        public string Header { get; set; }
        public string Version { get; set; }
        public float Tempo { get; set; }

        public List<Track> Tracks { get; set; }

        public Sample(string file)
        {
            this.File = file;
            this.Tracks = new List<Track>();
            // for US number format, used in .ToString()
            this._c = CultureInfo.CreateSpecificCulture("en-US");
        }

        public void AddHeader(byte[] data)
        {
            this.Header = ASCIIEncoding.ASCII.GetString(data);
        }

        public void AddVersion(byte[] data)
        {
            this.Version = ASCIIEncoding.ASCII.GetString(data);
        }

        public void AddTempo(byte[] data)
        {
            this.Tempo = BitConverter.ToSingle(data, 0);
        }

        public void AddTrack(Track t)
        {
            this.Tracks.Add(t);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.File);
            sb.AppendLine(String.Format("Saved with HW Version: {0}", this.Version));
            sb.AppendLine(String.Format("Tempo: {0}", this.Tempo.ToString(_c)));

            foreach (Track t in this.Tracks)
            {
                sb.Append(t.ToString());
            }

            return sb.ToString();
        }
    }

    public class Track
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Measure[] Measures { get; set; }

        public Track()
        {
            this.Measures = new Measure[4];
        }

        public void AddId(byte[] data)
        {
            this.Id = Convert.ToInt32(data[0]);
        }

        public void AddName(byte[] data)
        {
            this.Name = ASCIIEncoding.ASCII.GetString(data);
        }

        public void AddMeasures(byte[] data)
        {
            for (int i = 0; i < 4; i++)
            {
                Measure m = new Measure(
                    data.Skip(4 * i).Take(4).ToArray()
                    );
                this.Measures[i] = m;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("({0}) {1}\t|", this.Id.ToString(), this.Name));
            foreach (Measure m in this.Measures)
            {
                sb.Append(string.Format("{0}|", m.ToString()));
            }
            sb.Append("\n");
            return sb.ToString();
        }
    }

    public class Measure
    {
        private string Data;

        public Measure(byte[] data)
        {
            foreach (byte b in data)
            {
                if (b == 0)
                    this.Data += "-";
                if (b == 1)
                    this.Data += "x";
            }
        }

        public override string ToString()
        {
            return this.Data;
        }
    }
}
