using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing;
using System.Globalization;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;

using GH_IO.Serialization;

using Rhino.Geometry;
using Grasshopper.Kernel.Data;

namespace PTK
{
    public class ParticleSwarmOptimizationComponent : GH_Component
    {

        //-------フィールド（コンポーネントごとに保存する設定値）
        public int ParticleNum { get; set; }    //群の個体数
        public double W { get; set; }           //進行方向ベクトルの維持率　慣性定数
        public double C1 { get; set; }          //各個体でのベスト値方向ベクトルの影響率
        public double C2 { get; set; }          //群で共有するベスト値方向ベクトルの影響率
        public string SavePath { get; set; }    //群の保存パス

        public List<string> ParamSliderNames { get; private set; }      //パラメータとして使用するスライダーの名前
        //public List<string> FitnessSliderNames { get; private set; }    //目的関数として使用するパラムの名前
        //public bool ConstraintFitness { get; private set; }         //目的関数が制約内かどうか
        public List<ParetoSolution> ParetosOutput { get; set; } = new List<ParetoSolution>();
        public DataTree<double> FitOutput { get; set; } = new DataTree<double>();
        public DataTree<double> NowFitOutput { get; set; } = new DataTree<double>();

        public ParticleSwarmOptimizationComponent()
          : base("Particle Swarm Optimization", "PSO",
              "粒子群最適化を用いた多目的最適化を行うコンポーネント",
              CommonProps.category, CommonProps.subcate9)
        {
            //-------デフォルト値
            ParticleNum = 50;
            W = 0.4;
            C1 = 2.0;
            C2 = 2.0;
        }

        public override void CreateAttributes()
        {
            m_attributes = new Attributes_Custom(this);
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("ParamSliderNames", "SliName", "パラメータとして使用するスライダーの名前のリストを入力", GH_ParamAccess.list);
            pManager.AddNumberParameter("FitnessComponent", "FitComp", "目的関数の結果の値が格納されたコンポーネントに繋ぐ", GH_ParamAccess.list);
            pManager.AddBooleanParameter("ConstraintFitness", "ConstFit", "この入力がFalseの場合、目的関数は制約外として無効化される", GH_ParamAccess.item);
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.RegisterParam(new Param_ParetoSolution(), "ParetoSolutions", "PSs", "output",GH_ParamAccess.list);
            pManager.AddNumberParameter("ParetoFitness", "PareFit", "Pareto solution seems fitness", GH_ParamAccess.tree);
            pManager.AddNumberParameter("NowSwarmFitness", "NowFit", "Pareto solution seems fitness", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<String> paramSliderNames = new List<string>();
            if (!DA.GetDataList(0, paramSliderNames)) return;        //Get first input of component  
            if (paramSliderNames.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "ParamSliderNames : Required fields ");
                return;
            }
            ParamSliderNames = paramSliderNames;

            List<GH_ParetoSolution> paretos = ParetosOutput.ConvertAll(p => new GH_ParetoSolution(p));
            DA.SetDataList(0, paretos);
            DA.SetDataTree(1, FitOutput);
            DA.SetDataTree(2, NowFitOutput);
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.PSO;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("10e9cfa5-2d66-4cc1-91d2-f044dbe95a06"); }
        }

        public class Attributes_Custom : GH_ComponentAttributes
        {
            public Attributes_Custom(GH_Component owner) : base(owner)
            {
            }

            private Rectangle ButtonBounds { get; set; }    //Rectangle of button to be added

            //-------Change layout by overwriting setting
            protected override void Layout()
            {
                base.Layout();

                int buttonHeight = 22;      //The height of the button to be added
                Rectangle rec0 = GH_Convert.ToRectangle(Bounds);    //Component rectangle
                rec0.Height += buttonHeight;

                Rectangle rec1 = rec0;
                rec1.Y = rec1.Bottom - buttonHeight;
                rec1.Height = buttonHeight;
                rec1.Inflate(-2, -2);       //Offset button size

                Bounds = rec0;              //Change the whole shape
                ButtonBounds = rec1;        //Change shape of button
            }

            //-------Add button to component
            protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
            {
                base.Render(canvas, graphics, channel);
                if (channel == GH_CanvasChannel.Objects)
                {
                    GH_Capsule button = GH_Capsule.CreateTextCapsule(ButtonBounds, ButtonBounds, GH_Palette.Black, "PSO Run!", 2, 0);  //Add button
                    button.Render(graphics, Selected, Owner.Locked, false);
                    button.Dispose();
                }
            }
            //-------Addition of button clicking behavior
            public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Rectangle rec = ButtonBounds;
                    if (rec.Contains(System.Drawing.Point.Round(e.CanvasLocation)))
                    {
                        PSOOption.ShowGAForm((ParticleSwarmOptimizationComponent)Owner);        //Show Form
                        return GH_ObjectResponse.Handled;
                    }
                }
                return base.RespondToMouseDown(sender, e);
            }
        }

        //-------Save Fields Value
        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("ParticleNum", ParticleNum);
            writer.SetDouble("W", W);
            writer.SetDouble("C1", C1);
            writer.SetDouble("C2", C2);
            writer.SetString("SavePath", SavePath);

            return base.Write(writer);
        }

        //-------Load Fields Value
        public override bool Read(GH_IReader reader)
        {
            ParticleNum = reader.GetInt32("ParticleNum");
            W = reader.GetDouble("W");
            C1 = reader.GetDouble("C1");
            C2 = reader.GetDouble("C2");
            SavePath = reader.GetString("SavePath");

            return base.Read(reader);
        }
    }

    internal class DimensionVector
    {
        public List<decimal> Vecs { get; private set; }
        public DimensionVector()
        {
            Vecs = new List<decimal>();
        }
        public DimensionVector(List<decimal> _vecs)
        {
            Vecs = new List<decimal>(_vecs);
        }
        public DimensionVector(DimensionVector _dim)
        {
            Vecs = new List<decimal>(_dim.Vecs);    //deep
        }

        public static double Distance2Vectors(DimensionVector a, DimensionVector b)
        {
            //正確な距離は必要ないので平方根は適応していない
            if (a.Vecs.Count != b.Vecs.Count) throw new IndexOutOfRangeException();
            return a.Vecs.Zip(b.Vecs, (ai, bi) => Math.Pow((double)(ai - bi), 2)).Sum();
        }

        public string ToString(NumberFormatInfo nfi)
        {
            if (Vecs == null)
            {
                return "";
            }
            string output = "";
            foreach (decimal vec in Vecs)
            {
                output += vec.ToString(nfi) + '/';
            }
            return output;
        }
        public static DimensionVector FromString(string _str)
        {
            if(_str == "")
            {
                return new DimensionVector();
            }
            List<decimal> vecs = new List<decimal>();
            string[] strs = _str.Split('/');
            foreach(string s in strs)
            {
                if (s != "")
                {
                    vecs.Add(decimal.Parse(s));
                }
            }
            return new DimensionVector(vecs);
        }

        public static DimensionVector operator +(DimensionVector a, DimensionVector b)
        {
            if (a.Vecs.Count == b.Vecs.Count)
            {
                return new DimensionVector(a.Vecs.Zip(b.Vecs, (ai, bi) => ai + bi).ToList());
            }
            return null;
        }
        public static DimensionVector operator -(DimensionVector a, DimensionVector b)
        {
            if (a.Vecs.Count == b.Vecs.Count)
            {
                return new DimensionVector(a.Vecs.Zip(b.Vecs, (ai, bi) => ai - bi).ToList());
            }
            return null;
        }
        public static DimensionVector operator *(DimensionVector a, DimensionVector b)
        {
            if (a.Vecs.Count == b.Vecs.Count)
            {
                return new DimensionVector(a.Vecs.Zip(b.Vecs, (ai, bi) => ai * bi).ToList());
            }
            return null;
        }
        public static DimensionVector operator *(double a, DimensionVector b)
        {
            if (b.Vecs.Count != 0)
            {
                return new DimensionVector(b.Vecs.Select(bi => bi * (decimal)a).ToList());
            }
            return null;
        }
    }

    internal class ParticleArchive
    {
        public List<DimensionVector> Positions { get; private set; } = new List<DimensionVector>();
        public List<DimensionVector> Fitness { get; private set; } = new List<DimensionVector>();
        public List<DimensionVector> Sigmas { get; private set; } = new List<DimensionVector>();

        public ParticleArchive() { }

        public int ArchiveCount()
        {
            if (Positions.Count == Fitness.Count)
            {
                return Positions.Count;
            }
            return -1;
        }

        public DimensionVector GetRandomPosition()
        {
            Random rnd = new Random();
            return Positions[rnd.Next(Positions.Count-1)];
        }

        public DimensionVector GetNearBestPosition(Particle _particle)
        {
            if (_particle.IsEnableFitness == true)
            {
                return FindPositionNearSigma(_particle.Sigma);
            }
            else
            {
                return _particle.Position;
            }
        }

        private DimensionVector FindPositionNearSigma(DimensionVector _sigma)
        {
            double? minDistance = null;
            DimensionVector nearPosiiton = new DimensionVector();
            for(int i = 0; i< Sigmas.Count; i++)
            {
                double distance = DimensionVector.Distance2Vectors(Sigmas[i], _sigma);
                if (minDistance == null || minDistance > distance)
                {
                    minDistance = distance;
                    nearPosiiton = Positions[i];
                }
            }
            return nearPosiiton;
        }

        public bool TryAddBest(Particle _addParticle)
        {
            if (SearchParetoRank(_addParticle.Fitness) == 1)
            {
                Positions.Add(new DimensionVector(_addParticle.Position));
                Fitness.Add(new DimensionVector(_addParticle.Fitness));
                Sigmas.Add(new DimensionVector(_addParticle.Sigma));
                return true;
            }
            return false;
        }

        private int SearchParetoRank(DimensionVector _searchVec)
        {
            for(int i = Fitness.Count-1; i >=0; i--)
            {
                if (Fitness[i].Vecs.Count == _searchVec.Vecs.Count)
                {
                    var superiority = new List<bool>(Fitness[i].Vecs.Zip(_searchVec.Vecs, (ai, bi) =>  ai >= bi).ToList());
                    if (superiority.All(s=>s==true))    //過去のベストが優越された場合
                    {
                        Positions.RemoveAt(i);
                        Fitness.RemoveAt(i);
                        Sigmas.RemoveAt(i);
                    }else if (superiority.All(s => s == false))     //過去のベストに優越された場合
                    {
                        return 2;
                    }
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
            return 1;   //パレート解として保存可能
        }

        public string ToString(NumberFormatInfo _nfi)
        {
            if (Positions.Count == 0)
            {
                return "";
            }
            string output = "";
            foreach (DimensionVector posi in Positions)
            {
                output += posi.ToString(_nfi) + ':';
            }
            output += ';';
            foreach (DimensionVector fit in Fitness)
            {
                output += fit.ToString(_nfi) + ':';
            }
            output += ';';
            foreach (DimensionVector sig in Sigmas)
            {
                output += sig.ToString(_nfi) + ':';
            }
            return output;
        }

        public static ParticleArchive FromString(string _str)
        {
            if (_str == "")
            {
                return null;
            }
            ParticleArchive arc = new ParticleArchive();
            string[] member = _str.Split(';');
            string[] posi = member[0].Split(':');
            foreach(string str in posi)
            {
                arc.Positions.Add(DimensionVector.FromString(str));
            }
            string[] fit = member[1].Split(':');
            foreach (string str in fit)
            {
                arc.Fitness.Add(DimensionVector.FromString(str));
            }
            string[] sig = member[2].Split(':');
            foreach (string str in sig)
            {
                arc.Sigmas.Add(DimensionVector.FromString(str));
            }
            return arc;
        }
    }

    internal class Particle : ICloneable
    {
        public DimensionVector Position { get; private set; } = new DimensionVector();
        public DimensionVector MoveVector { get; private set; } = new DimensionVector();
        public DimensionVector Fitness { get; private set; } = new DimensionVector();
        public bool IsEnableFitness { get; set; } = false;
        public DimensionVector Sigma { get; private set; } = new DimensionVector();

        public ParticleArchive ParticleBests { get; private set; } = new ParticleArchive();    //支配されていない過去のベスト値

        public Particle() { }


        //-------Clamp position range from 0.0M to 1.0M
        #region //Operation method of position
        private decimal ClampPosition(decimal _position)
        {
            return Math.Min(Math.Max(_position, 0.0M), 1.0M);
        }
        public void SetPositions(List<decimal> _positions)
        {
            Position.Vecs.Clear();
            foreach (decimal p in _positions)
            {
                Position.Vecs.Add(ClampPosition(p));
            }
        }
        public void SetPositions(DimensionVector _positions)
        {
            SetPositions(_positions.Vecs);
        }
        public void SetPosition(decimal _position, int _loc)
        {
            Position.Vecs[_loc] = ClampPosition(_position);
        }
        public List<decimal> GetPositions()
        {
            return Position.Vecs;
        }
        public decimal GetPosition(int _loc)
        {
            return Position.Vecs[_loc];
        }
        public void AddPosition(decimal _position)
        {
            Position.Vecs.Add(ClampPosition(_position));
        }
        #endregion

        #region //Operation method of MoveVector
        public void SetMoveVectors(DimensionVector _moveVecs)
        {
            MoveVector = _moveVecs;
        }
        public void AddMoveVectors(decimal _moveVec)
        {
            MoveVector.Vecs.Add(_moveVec);
        }
        #endregion

        #region //Operation method of Fitness
        public void SetFitness(DimensionVector _fitness)
        {
            Fitness = _fitness;
            if (Fitness.Vecs.Count != 0)
            {
                CalculateSigma();
            }
        }
        #endregion

        #region //Operation method of ParticleBests
        public void SetParticleBests(ParticleArchive _particleBests)
        {
            ParticleBests = _particleBests;
        }
        #endregion

        #region //Operation method of Sigma
        public void SetSigma(DimensionVector _sigma)
        {
            Sigma = _sigma;
        }
        #endregion



        private void CalculateSigma()
        {
            var fits = Fitness.Vecs;
            decimal sum = fits.Sum(f => f * f);
            Sigma.Vecs.Clear();
            for (int i = 0; i < fits.Count - 1; i++)
            {
                Sigma.Vecs.Add(1.0M / sum * (fits[i] * fits[i] - fits[i + 1] * fits[i + 1]));
            }
            Sigma.Vecs.Add(1.0M / sum * (fits[fits.Count - 1] * fits[fits.Count - 1] - fits[0] * fits[0]));
        }
        public bool TryAddParticleBests(Particle _particle)
        {
            return ParticleBests.TryAddBest(_particle);
        }
        #region //Method for deep copy
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public Particle CopyIndividual()
        {
            Particle ind = (Particle)this.Clone();
            return ind;
        }
        public Particle DeepCopyIndividual()
        {
            Particle ind = (Particle)this.Clone();
            return ind;
        }
        #endregion
    }

    internal class Swarm
    {
        public List<Particle> Particles { get; set; } = new List<Particle>();           //集団
        public ParticleArchive GroupBests { get; set; } = new ParticleArchive();      //過去のベスト値
        private Random rnd = new Random();

        public Swarm() { }


        public void UpdateParticlePosition(Particle _p)
        {
            DimensionVector newMoveVector = PSOOption.W * _p.MoveVector +
                PSOOption.C1 * rnd.NextDouble() * (_p.ParticleBests.GetNearBestPosition(_p) - _p.Position) +
                PSOOption.C2 * rnd.NextDouble() * (GroupBests.GetNearBestPosition(_p) - _p.Position);

            _p.SetPositions(_p.Position + newMoveVector);
            _p.SetMoveVectors(newMoveVector);
        }

        public void TryAddGroupBests()
        {
            foreach(Particle particle in Particles.Where(p=>p.IsEnableFitness==true))
            {
                GroupBests.TryAddBest(particle);    //グループのパレートに追加試行
                particle.TryAddParticleBests(particle);     //個別のパレートに追加試行
            }
        }

    }


    internal static class PSOOption
    {
        public static int ParticleNum { get; private set; }
        public static double W { get; private set; }
        public static double C1 { get; private set; }
        public static double C2 { get; private set; }
        public static string SavePath { get; private set; }


        public static ParticleSwarmOptimizationComponent Comp { get; private set; }   //Parent component
        public static GH_Document GhDoc { get; private set; }                   //Reference to Grasshopper's document

        public static PSOForm psoFrom;

        public static List<string> ParamSliderNames { get; set; }        //Name of slider to use as gene
        public static List<string> FitnessNames { get; set; }             //Name indicating the value of the objective function to be optimized
        public static Grasshopper.Kernel.Special.GH_NumberSlider[] paramSliders;        //Reference of param slider
        public static GH_Param<Grasshopper.Kernel.Types.GH_Number> fitnessParam;        //Reference to fitness parameter 
        public static GH_Param<Grasshopper.Kernel.Types.GH_Boolean> constraintParam;    //Reference to constraint parameter 

        public static int paramLength;                                   //Length of params in individuals


        //-------Initialize
        public static void SetPSOOption(ParticleSwarmOptimizationComponent _comp)
        {
            Comp = _comp;
            GhDoc = Comp.OnPingDocument();
        }

        //-------Reflect setting value of genetic algorithm from form
        public static bool ReflectValueFromForm()
        {
            try
            {
                ParticleNum = psoFrom.GetParticleNum();
                W = psoFrom.GetW();
                C1 = psoFrom.GetC1();
                C2 = psoFrom.GetC2();
                SavePath = psoFrom.GetSavePath();

                ParamSliderNames = Comp.ParamSliderNames;

                //-------Search reference to slider
                var localSliders = new List<Grasshopper.Kernel.Special.GH_NumberSlider>();
                paramLength = 0;
                foreach (String sliderName in ParamSliderNames)
                {
                    bool IsFound = false;
                    foreach (IGH_DocumentObject obj in GhDoc.Objects) //Exploring components in the Grasshopper
                    {
                        var slider = obj as Grasshopper.Kernel.Special.GH_NumberSlider;
                        if (slider == null) continue;
                        if (slider.NickName == sliderName)  //If the slider name is a search name
                        {
                            localSliders.Add(slider);
                            paramLength++;
                            IsFound = true;
                            break;
                        }
                    }
                    if (!IsFound)
                        MessageBox.Show("Could not find the slider name:" + sliderName);
                }
                paramSliders = localSliders.ToArray();
                if (paramLength == 0)
                    throw new InvalidOperationException("Could no find ParamSliderNames");

                //-------Search reference to fitness
                fitnessParam = Comp.Params.Input[1] as Grasshopper.Kernel.GH_Param<Grasshopper.Kernel.Types.GH_Number>;
                if (fitnessParam == null)
                    throw new InvalidOperationException("An inappropriate value was entered as input for fitness");
                FitnessNames = Comp.Params.Input[1].Sources.ToList().ConvertAll(s => s.NickName);
                if (FitnessNames.Count != FitnessNames.Distinct().Count())
                    throw new InvalidOperationException("The nickname of the component set in the objective function contains duplicates");

                //-------Search reference to fitness
                constraintParam = Comp.Params.Input[2] as Grasshopper.Kernel.GH_Param<Grasshopper.Kernel.Types.GH_Boolean>;
                if (constraintParam == null)
                    throw new InvalidOperationException("An inappropriate value was entered as input for constraint");

                return true;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        //-------Show Form
        public static void ShowGAForm(ParticleSwarmOptimizationComponent _comp)
        {
            _comp.ExpireSolution(true);
            if (_comp.SavePath == null)
                _comp.SavePath = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + System.IO.Path.GetFileNameWithoutExtension(Grasshopper.Instances.ActiveCanvas.Document.FilePath) + ".csv";        //Select Desktop
            SetPSOOption(_comp);
            if (psoFrom == null)
            {
                psoFrom = new PSOForm(_comp);     //Materialize the form
                GH_WindowsFormUtil.CenterFormOnEditor(psoFrom, true);
            }
            psoFrom.Show();                  //
            psoFrom.SetOptionFromComp();     //Reflect the eigenvalue of the component in the form
        }
    }

    internal static class ParticleSwarmOptimization
    {
        private static Swarm Swarm;
        private static Random rnd = new Random();
        public static bool IsAbort { get; set; }        //Interrupt flag
        private static int age;                         //Current generation number
        private static int particleIndex;               //Individual index for loop


        //-------Reset and Initialize
        public static bool ResetPSO()
        {
            if (PSOOption.ReflectValueFromForm())
            {
                Swarm = new Swarm();
                age = 0;
                particleIndex = -1;
                rnd = new Random();
                return true;
            }
            return false;
        }

        //-------Resume
        public static bool ResumeRun()
        {
            if (InputCSVSwarm(out Swarm))     //Extract saved data from CSV
            {
                return true;
            }
            else       //When loading CSV failed or the saved setting value is different from the current setting
            {
                WriteLogForm("Failed to load CSV, Execute from the beginning");
                Swarm = new Swarm();
                age = 0;
                particleIndex = -1;
                return false;
            }
        }

        public static void PSORun()
        {
            try
            {
                if (IsAbort)
                {
                    if (true)
                    {
                        WriteLogForm("Stop GA");
                    }
                    return;  //Stop Loop
                }
                if (particleIndex == -1)  //Only the first loop exception
                {
                    CreateRandomSwarm(out Swarm);     //Set genes of current generation individuals as random
                    WriteLogForm("Initialised generation");
                    particleIndex = 0;
                    SetSlider(Swarm.Particles[particleIndex]);
                }
                else if (particleIndex == PSOOption.ParticleNum)   //If one generation finish
                {

                    Swarm.TryAddGroupBests();

                    OutputSwarm();
                    OutpuCSVSwarm(Swarm, false);
                    //ここに終了条件
                    //

                    for (int i = 0; i < Swarm.Particles.Count; i++)
                    {
                        Swarm.UpdateParticlePosition(Swarm.Particles[i]);
                    }

                    WriteLogForm("Age:" + age.ToString() + " End");
                    age++;
                    particleIndex = 0;
                    SetSlider(Swarm.Particles[particleIndex]);
                }
                else
                {
                    DimensionVector fitVal;
                    if (GetFitnessVal(out fitVal))
                    {
                        Swarm.Particles[particleIndex].IsEnableFitness = true;
                        Swarm.Particles[particleIndex].SetFitness(fitVal);      //Set gene to slider and get fitness
                    }
                    else
                    {
                        Swarm.Particles[particleIndex].IsEnableFitness = false;
                        Swarm.Particles[particleIndex].SetFitness(fitVal);
                    }
                    particleIndex++;
                    if (particleIndex < PSOOption.ParticleNum)
                    {
                        SetSlider(Swarm.Particles[particleIndex]);
                    }
                }
            }

            catch (Exception e)
            {
                //-------Error handling
                MessageBox.Show(e.Message);
                if (PSOOption.psoFrom != null)
                {
                    WriteLogForm("GA is terminated because an Unexpected Error occurred");
                    PSOOption.psoFrom.TransitionStopingState();
                }
                return;
            }
            Task.Run(() => PSORun());
        }


        //-------Initialize Swarm as random
        private static void CreateRandomSwarm(out Swarm _swarm)
        {
            _swarm = new Swarm();
            for (int i = 0; i < PSOOption.ParticleNum; i++)
            {
                _swarm.Particles.Add(new Particle());
                for (int j = 0; j < PSOOption.paramLength; j++)
                {
                    _swarm.Particles.Last().AddPosition((decimal)rnd.NextDouble());  //Fill one gene randomly
                    _swarm.Particles.Last().AddMoveVectors((decimal)rnd.NextDouble());  //長さ要確認
                }
            }
        }

        //-------Reflect the value in the slider
        private static void SetSlider(Particle _par)
        {
            Grasshopper.Instances.DocumentEditor.Invoke(new Action(() =>
            {
                //-------Adapted to slider for each gene
                for (int i = 0; i < PSOOption.paramLength; i++)
                {
                    decimal sliMin = PSOOption.paramSliders[i].Slider.Minimum;
                    PSOOption.paramSliders[i].SetSliderValue(_par.GetPosition(i) * (PSOOption.paramSliders[i].Slider.Maximum - sliMin) + sliMin);
                }
                PSOOption.GhDoc.NewSolution(false);    //Recalculate uncalculated parts
            }));
        }

        //-------Acquire fitness from Position
        private static bool GetFitnessVal(out DimensionVector _fitVal)
        {
            _fitVal = new DimensionVector();

            if (PSOOption.constraintParam.VolatileData.get_Branch(0)[0] is Grasshopper.Kernel.Types.GH_Boolean constraint)
            {
                if (constraint.Value == false)
                {
                    return false;
                }
            }

            try
            {
                var fitList = PSOOption.fitnessParam.VolatileData.get_Branch(0);
                foreach (var item in fitList)
                {
                    if (item is Grasshopper.Kernel.Types.GH_Number num)
                    {
                        _fitVal.Vecs.Add((decimal)num.Value);
                    }
                }
                return true;
            }
            catch
            {
                _fitVal = new DimensionVector();
                return false;
            }
        }

        //-------出力に現在のパレート解と現世代の位置をセット
        private static void OutputSwarm()
        {
            //現世代でのパレート解を変換
            List<ParetoSolution> paretos = new List<ParetoSolution>();

            for(int i = 0; i < Swarm.GroupBests.ArchiveCount(); i++)
            {
                ParetoSolution pareto = new ParetoSolution();
                if (Swarm.GroupBests.Positions[i].Vecs.Count == PSOOption.ParamSliderNames.Count)
                {
                    for(int j = 0; j< PSOOption.ParamSliderNames.Count; j++)
                    {
                        pareto.AddPosition(PSOOption.ParamSliderNames[j], Swarm.GroupBests.Positions[i].Vecs[j]);
                    }
                }
                if (Swarm.GroupBests.Fitness[i].Vecs.Count == PSOOption.FitnessNames.Count)
                {
                    for(int j = 0; j < PSOOption.FitnessNames.Count; j++)
                    {
                        pareto.AddFitness(PSOOption.FitnessNames[j], Swarm.GroupBests.Fitness[i].Vecs[j]);
                    }
                }
                paretos.Add(pareto);
            }
            

            DataTree<double> fitTree = new DataTree<double>();
            int path = 0;
            foreach(DimensionVector fit in Swarm.GroupBests.Fitness)
            {
                fitTree.AddRange(fit.Vecs.ConvertAll(d => (double)d), new Grasshopper.Kernel.Data.GH_Path(path));
                path++;
            }

            DataTree<double> nowFitTree = new DataTree<double>();
            path = 0;
            foreach(DimensionVector fit in Swarm.Particles.Where(p=>p.IsEnableFitness==true).ToList().ConvertAll(p => p.Fitness))
            {
                nowFitTree.AddRange(fit.Vecs.ConvertAll(d => (double)d), new Grasshopper.Kernel.Data.GH_Path(path));
                path++;
            }

            PSOOption.Comp.ParetosOutput = paretos;
            PSOOption.Comp.FitOutput = fitTree;
            PSOOption.Comp.NowFitOutput = nowFitTree;
        }

        //-------Save generation with CSV
        private static void OutpuCSVSwarm(Swarm _swarm, bool _IsAdd)
        {
            string csvFilePath = PSOOption.SavePath;
            System.IO.StreamWriter sw = new System.IO.StreamWriter(csvFilePath, _IsAdd);
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            foreach (Particle part in _swarm.Particles)
            {
                sw.Write(age.ToString(nfi) + ',');                    //Age
                sw.Write(part.Position.ToString(nfi) + ',');          //Position
                sw.Write(part.MoveVector.ToString(nfi) + ',');        //MoveVector
                sw.Write(part.Fitness.ToString(nfi) + ',');           //Fitness
                //sw.Write(part.Sigma.ToString(nfi) + ',');             //Sigma
                sw.Write(part.ParticleBests.ToString(nfi) + ',');     //ParticleBests
                sw.Write(part.IsEnableFitness.ToString(nfi) + ',');   //Enable

                sw.Write("\r\n");
            }
            sw.Write("Swarm," + _swarm.GroupBests.ToString(nfi) + ',');     //GroupBests
            sw.Write("\r\n");

            sw.Close();
        }

        //-------Read generation from CSV
        private static bool InputCSVSwarm(out Swarm _swarm)
        {
            _swarm = new Swarm();
            string csvFilePath = PSOOption.SavePath;
            if (!System.IO.File.Exists(csvFilePath))    //When the file can not be found
            {
                WriteLogForm("Not Find CSV File!");
                return false;
            }

            System.IO.StreamReader sr = new System.IO.StreamReader(csvFilePath, true);

            age = 0;
            int ParticleCount = 0;
            bool IsAgeReaded = false;

            try
            {
                while (sr.EndOfStream == false)
                {
                    string line = sr.ReadLine();
                    string[] fields = line.Split(',');

                    if(fields[0] != "Swarm")
                    {
                        ParticleCount++;
                        Particle part = new Particle();

                        if (!IsAgeReaded)
                        {
                            age = int.Parse(fields[0]);     //Read Age only once
                            IsAgeReaded = true;
                        }
                        part.SetPositions(DimensionVector.FromString(fields[1]));
                        part.SetMoveVectors(DimensionVector.FromString(fields[2]));
                        part.SetFitness(DimensionVector.FromString(fields[3]));
                        part.SetParticleBests(ParticleArchive.FromString(fields[4]));
                        part.IsEnableFitness = bool.Parse(fields[5]);
                        _swarm.Particles.Add(part);
                    }else if(fields[0] == "Swarm")
                    {
                        _swarm.GroupBests = ParticleArchive.FromString(fields[1]);
                    }
                }
                if (ParticleCount != PSOOption.ParticleNum) //Number of individuals is different from preserved
                {
                    WriteLogForm("ParticleNum is different from CSV File!");
                    return false;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                sr.Close();
            }
            return true;
        }

        //-------Write String to form log
        private static void WriteLogForm(string _logtext)
        {
            PSOOption.psoFrom.WriteLog(_logtext);
        }

    }
}
