using GH_IO.Serialization;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

//using Rhino.Geometry;

namespace PTK.Optimization
{
    /// <summary>
    /// Parameter optimization component using genetic algorithm
    /// 　Main Terms
    /// 　・Gene          Parameter which is a variable of gene
    /// 　・Fitness       Evaluation value obtained by gene list
    /// 　・Individual    It has a list of genes set to a certain value and evaluation values accordingly
    /// 　・Generation    Have multiple individuals
    /// 　・Selection     Choose individuals with high fitness from among generations. Reproduction of selection
    /// 　・Crossover     Produce individuals that will be children of the next generation from genetic information of multiple individuals
    /// 　・Mutation      Add irregular changes to individual genes with low probability
    /// </summary>

    //-----------------------------------
    //  Class of component body
    //-----------------------------------
    public class PTK_GeneticAlgorithm : GH_Component
    {
        //-------Field ( Variable for saving for each component placed )
        public int IndividualNum { get; set; }          //Number of individuals within one generation
        public double EliteRate { get; set; }           //Probability of elite selection to generation
        public double MutationRate { get; set; }        //Probability of mutation by genetic manipulation
        public bool IsMinimize { get; set; }            //Whether to make fitness minimum

        public bool IsEnableMaxGene { get; set; }       //Is the censoring by number of generations effective
        public int MaxGeneration { get; set; }          //Maximum number of generations to change
        public bool IsEnableContinuation { get; set; }  //Whether discontinuation by succession of the best individuals is effective
        public int ContinuationNum { get; set; }        //Number of consecutive number of best individuals who abolish generation alternation
        public string SavePath { get; set; }            //Storage location when interrupted

        public List<string> GeneSliderNames { get; private set; }   //Name of slider to use as gene


        //-------Overview
        public PTK_GeneticAlgorithm()
          : base("GeneticAlgorithmComponent", "GA",
              "The sliders are regarded as genes and the solution is searched for by genetic algorithm so that the fitness becomes the best.",
              CommonProps.category, CommonProps.subcate9)
        {
            //-------Default value
            IndividualNum = 50;
            EliteRate = 0.20;
            MutationRate = 0.01;
            IsMinimize = true;
            IsEnableMaxGene = true;
            MaxGeneration = 100;
            IsEnableContinuation = true;
            ContinuationNum = 30;
        }

        //-------Override appearance of component
        public override void CreateAttributes()
        {
            m_attributes = new Attributes_Custom(this);
        }

        //-------Input
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("GeneSliderNames", "SliName", "Slider names to be manipulated for genes", GH_ParamAccess.list);
            pManager.AddNumberParameter("FitnessComponent", "FitComp", "Please connect to primitive component showing fitness", GH_ParamAccess.item);
        }

        //-------Output
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        //-------Functions to be executed each time in a component
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<String> geneSliderNames = new List<string>();
            if (!DA.GetDataList(0, geneSliderNames)) return;        //Get first input of component  
            if (geneSliderNames.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "GeneSliderNames : Required fields ");
                return;
            }
            GeneSliderNames = geneSliderNames;
        }

        //-------Icon setting
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Resources.GA;
            }
        }

        //-------GUID
        public override Guid ComponentGuid
        {
            get { return new Guid("a1ee6b21-6735-495f-b139-f9cb01c8e8b9"); }
        }

        //-------Appearance customization of components
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
                    GH_Capsule button = GH_Capsule.CreateTextCapsule(ButtonBounds, ButtonBounds, GH_Palette.Black, "GA Run!", 2, 0);  //Add button
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
                        GeneAlgoOption.ShowGAForm((PTK_GeneticAlgorithm)Owner);        //Show Form
                        return GH_ObjectResponse.Handled;
                    }
                }
                return base.RespondToMouseDown(sender, e);
            }
        }

        //-------Save Fields Value
        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("IndividualNum", IndividualNum);
            writer.SetDouble("EliteRate", EliteRate);
            writer.SetDouble("MutationRate", MutationRate);
            writer.SetBoolean("IsMinimize", IsMinimize);
            writer.SetBoolean("IsEnableMaxGene", IsEnableMaxGene);
            writer.SetInt32("MaxGeneration", MaxGeneration);
            writer.SetBoolean("IsEnableContinuation", IsEnableContinuation);
            writer.SetInt32("ContinuationNum", ContinuationNum);
            writer.SetString("SavePath", SavePath);
            return base.Write(writer);
        }

        //-------Load Fields Value
        public override bool Read(GH_IReader reader)
        {
            IndividualNum = reader.GetInt32("IndividualNum");
            EliteRate = reader.GetDouble("EliteRate");
            MutationRate = reader.GetDouble("MutationRate");
            IsMinimize = reader.GetBoolean("IsMinimize");
            IsEnableMaxGene = reader.GetBoolean("IsEnableMaxGene");
            MaxGeneration = reader.GetInt32("MaxGeneration");
            IsEnableContinuation = reader.GetBoolean("IsEnableContinuation");
            ContinuationNum = reader.GetInt32("ContinuationNum");
            SavePath = reader.GetString("SavePath");
            return base.Read(reader);
        }
    }

    //--------------------------------------------------------
    //  Class for storing set values of genetic algorithm
    //--------------------------------------------------------
    internal static class GeneAlgoOption
    {
        public static PTK_GeneticAlgorithm Comp { get; private set; }    //Parent component
        public static GH_Document GhDoc { get; private set; }                   //Reference to Grasshopper's document

        public static GAForm gaForm;                                    //Form for managing execution of genetic algorithm

        public static int IndividualNum { get; private set; }           //Number of individuals within one generation
        public static double EliteRate { get; private set; }            //Probability of elite selection to generation
        public static double MutationRate { get; private set; }         //Probability of mutation by genetic manipulation
        public static bool IsMinimize { get; private set; }             //Whether to make fitness minimum

        public static bool IsEnableMaxGene { get; private set; }        //Is the censoring by number of generations effective?
        public static int MaxGeneration { get; private set; }           //Maximum number of generations to change
        public static bool IsEnableContinuation { get; private set; }   //Whether discontinuation by succession of the best individuals is effective
        public static int ContinuationNum { get; private set; }         //Number of consecutive number of best individuals who abolish generation alternation
        public static string SavePath { get; private set; }

        public static List<string> GeneSliderNames { get; set; }        //Name of slider to use as gene
        public static string FitName { get; set; }                      //Name indicating the value of the objective function to be optimized
        public static Grasshopper.Kernel.Special.GH_NumberSlider[] geneSliders;        //Reference of gene slider
        public static GH_Param<Grasshopper.Kernel.Types.GH_Number> fitnessParam;       //Reference to fitness parameter 

        public static int geneLength;                                   //Length of genes in individuals


        //-------Initialize
        public static void SetGAOption(PTK_GeneticAlgorithm _comp)
        {
            Comp = _comp;
            GhDoc = Comp.OnPingDocument();
        }

        //-------Reflect setting value of genetic algorithm from form
        public static bool ReflectValueFromForm()
        {
            try
            {
                IndividualNum = gaForm.GetIndivitualNum();
                EliteRate = gaForm.GetEliteRate();
                MutationRate = gaForm.GetMutationRate();
                IsMinimize = gaForm.GetIsFitnessMinimize();

                IsEnableMaxGene = gaForm.GetIsMaxGenerationChecked();
                MaxGeneration = gaForm.GetMaxGeneration();
                IsEnableContinuation = gaForm.GetIsContinuationChecked();
                ContinuationNum = gaForm.GetContinuation();
                SavePath = gaForm.GetSavePath();

                GeneSliderNames = Comp.GeneSliderNames;

                //-------Search reference to slider
                var localSliders = new List<Grasshopper.Kernel.Special.GH_NumberSlider>();
                geneLength = 0;
                foreach (String sliderName in GeneSliderNames)
                {
                    bool IsFound = false;
                    foreach (IGH_DocumentObject obj in GhDoc.Objects) //Exploring components in the Grasshopper
                    {
                        var slider = obj as Grasshopper.Kernel.Special.GH_NumberSlider;
                        if (slider == null) continue;
                        if (slider.NickName == sliderName)  //If the slider name is a search name
                        {
                            localSliders.Add(slider);
                            geneLength++;
                            IsFound = true;
                            break;
                        }
                    }
                    if (!IsFound)
                        MessageBox.Show("Could not find the slider name:" + sliderName);
                }
                geneSliders = localSliders.ToArray();
                if (geneLength == 0)
                    throw new InvalidOperationException("Could no find GeneSliderNames");

                //-------Search reference to fitness
                fitnessParam = Comp.Params.Input[1] as Grasshopper.Kernel.GH_Param<Grasshopper.Kernel.Types.GH_Number>;
                if (fitnessParam == null)
                    throw new InvalidOperationException("An inappropriate value was entered as input for fitness");

                return true;
            }
            catch
            {
                return false;
            }
        }

        //-------Show Form
        public static void ShowGAForm(PTK_GeneticAlgorithm _comp)
        {
            _comp.ExpireSolution(true);
            if (_comp.SavePath == null)
                _comp.SavePath = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\" + System.IO.Path.GetFileNameWithoutExtension(Grasshopper.Instances.ActiveCanvas.Document.FilePath) + ".csv";        //Select Desktop
            SetGAOption(_comp);
            if (gaForm == null)
            {
                gaForm = new GAForm(_comp);     //Materialize the form
                GH_WindowsFormUtil.CenterFormOnEditor(gaForm, true);
            }
            gaForm.Show();                  //
            gaForm.SetOptionFromComp();     //Reflect the eigenvalue of the component in the form
        }
    }

    //--------------------------------------------------------
    //  Individual class with genetic information
    //--------------------------------------------------------
    internal class Individual : ICloneable
    {
        private List<decimal> genes;          //gene = 0.0～1.0
        public decimal Fitness { get; set; }  //
        public bool IsEnableFitness { get; set; }

        #region //Constructor
        public Individual(List<decimal> _genes, decimal _fitness = 0.0M)
        {
            genes = _genes;
            Fitness = _fitness;
            IsEnableFitness = true;
        }
        public Individual()
        {
            genes = new List<decimal>();
            Fitness = 0.0M;
            IsEnableFitness = true;
        }
        #endregion

        //-------Clamp gene range from 0.0M to 1.0M
        private decimal ClampGene(decimal _gene)
        {
            return Math.Min(Math.Max(_gene, 0.0M), 1.0M);
        }

        #region //Operation method of genes
        public void SetGenes(List<decimal> _genes)
        {
            genes.Clear();
            foreach (decimal g in _genes)
            {
                genes.Add(ClampGene(g));
            }
        }
        public void SetGene(decimal _gene, int _loc)
        {
            genes[_loc] = ClampGene(_gene);
        }
        public List<decimal> GetGenes()
        {
            return genes;
        }
        public decimal GetGene(int _loc)
        {
            return genes[_loc];
        }
        public void AddGene(decimal _gene)
        {
            genes.Add(ClampGene(_gene));
        }
        public static Individual operator +(Individual a, Individual b)
        {
            Individual added = new Individual();
            for (int i = 0; i < a.genes.Count(); i++)
            {
                added.AddGene(a.genes[i] + b.genes[i]);
            }
            return added;
        }
        public static Individual operator -(Individual a, Individual b)
        {
            Individual subtracted = new Individual();
            for (int i = 0; i < a.genes.Count(); i++)
            {
                subtracted.AddGene(a.genes[i] - b.genes[i]);
            }
            return subtracted;
        }
        public static Individual operator *(double a, Individual b)
        {
            Individual multiplied = new Individual();
            for (int i = 0; i < b.genes.Count(); i++)
            {
                multiplied.AddGene((decimal)a * b.genes[i]);
            }
            return multiplied;
        }
        #endregion

        #region //Method for deep copy
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public Individual CopyIndividual()
        {
            Individual ind = (Individual)this.Clone();
            return ind;
        }
        public Individual DeepCopyIndividual()
        {
            Individual ind = (Individual)this.Clone();
            ind.genes = new List<decimal>(this.genes);
            return ind;
        }
        #endregion
    }

    //--------------------------------------------------------
    //  Class to execute genetic algorithm
    //--------------------------------------------------------
    internal static class GeneticAlgo
    {
        private static List<Individual> nowGeneration;     //
        private static List<Individual> nextGeneration;    //
        public static bool IsAbort { get; set; }           //Interrupt flag

        private static int age;                //Current generation number
        private static int individualIndex;    //Individual index for loop
        private static Random rnd;             //Shared random number source

        private static Individual bestIndividual;   //Individuals with the best Fitness
        private static int continuationCount;       //Continuous number of best individuals

        //-------Reset and Initialize
        public static bool ResetGeneticAlgo()
        {
            if (GeneAlgoOption.ReflectValueFromForm())
            {
                nowGeneration = new List<Individual>();
                nextGeneration = new List<Individual>();
                age = 0;
                individualIndex = -1;
                continuationCount = 0;
                rnd = new Random();
                return true;
            }
            return false;
        }

        //-------Resume
        public static bool ResumeRun()
        {
            if (InputCSVGeneration(out nextGeneration))     //Extract saved data from CSV
            {
                individualIndex = GeneAlgoOption.IndividualNum;
                bestIndividual = GeneAlgoOption.IsMinimize ?
                       (Individual)nextGeneration.Where(i => i.IsEnableFitness == true).OrderBy(i => i.Fitness).ElementAt(0).DeepCopyIndividual()
                       : (Individual)nextGeneration.Where(i => i.IsEnableFitness == true).OrderByDescending(i => i.Fitness).ElementAt(0).DeepCopyIndividual();    //Get the best individual

                return true;
            }
            else       //When loading CSV failed or the saved setting value is different from the current setting
            {
                WriteLogForm("Failed to load CSV, Execute from the beginning");
                nextGeneration.Clear();
                age = 0;
                individualIndex = -1;
                return false;
            }
        }

        //-------Main loop
        public static void GARun()
        {
            try
            {
                if (IsAbort)
                {
                    if (nowGeneration != null && nowGeneration.Count != 0)
                    {
                        SetExcellenceIndividual(nowGeneration);     //Set the best individual
                        WriteLogForm("Stop GA");
                    }
                    return;  //Stop Loop
                }
                if (individualIndex == -1)  //Only the first loop exception
                {
                    CreateRandomGeneration(out nextGeneration);     //Set genes of current generation individuals as random
                    WriteLogForm("Initialised generation");
                    individualIndex = 0;
                    SetSlider(nextGeneration[individualIndex]);
                }
                else if (individualIndex == GeneAlgoOption.IndividualNum)   //If one generation finish
                {
                    nowGeneration.Clear();
                    nowGeneration.AddRange(nextGeneration.Select(i => (Individual)i.DeepCopyIndividual()));     //Copy generations and shift

                    //-------Check best fitness update
                    Individual tmpBestIndividual = GeneAlgoOption.IsMinimize ?
                        (Individual)nowGeneration.Where(i => i.IsEnableFitness == true).OrderBy(i => i.Fitness).ElementAt(0).DeepCopyIndividual()
                        : (Individual)nowGeneration.Where(i => i.IsEnableFitness == true).OrderByDescending(i => i.Fitness).ElementAt(0).DeepCopyIndividual();    //Get the best individual

                    if (age == 0 || (GeneAlgoOption.IsMinimize ? bestIndividual.Fitness > tmpBestIndividual.Fitness : bestIndividual.Fitness < tmpBestIndividual.Fitness))    //Does it apply to the update condition of the best value
                    {
                        bestIndividual = tmpBestIndividual;    //Update best value
                        continuationCount = 0;
                    }
                    else
                    {
                        continuationCount++;
                    }

                    OutputFitnessVal(nowGeneration, true);                  //Display fitness in log
                    DrawFitnessVal(nowGeneration, continuationCount == 0);  //Draw fitness on graph
                    OutpuCSVGeneration(nowGeneration, false);               //Save current generation as CSV

                    //-------When the termination condition is reached
                    if ((GeneAlgoOption.IsEnableMaxGene && age >= GeneAlgoOption.MaxGeneration) || (GeneAlgoOption.IsEnableContinuation && continuationCount >= GeneAlgoOption.ContinuationNum))
                    {
                        SetExcellenceIndividual(nowGeneration);     //Set the best individual
                        WriteLogForm("End GA");
                        GeneAlgoOption.gaForm.TransitionStopingState();
                        return;
                    }
                    else
                    {
                        nowGeneration = NormaliseFitnessGeneration(nowGeneration);  //Normalize fitness

                        //-------Generate the next generation of individuals
                        nextGeneration.Clear();
                        for (int i = 0; i < GeneAlgoOption.IndividualNum; i++)
                        {
                            Individual nextInd;
                            if (rnd.NextDouble() < GeneAlgoOption.EliteRate)
                            {
                                nextInd = SelectionIndividual(nowGeneration);   //Elite extraction
                            }
                            else
                            {
                                nextInd = CrossoverGene_BLXa(SelectionIndividual(nowGeneration), SelectionIndividual(nowGeneration));   //Crossover
                                nextInd = MutationGene(nextInd, GeneAlgoOption.MutationRate);                               //Mutation
                            }
                            nextGeneration.Add(nextInd);   //Add child individuals to the next generation
                        }
                        WriteLogForm("Age:" + age.ToString() + " End");
                        age++;
                        individualIndex = 0;
                        SetSlider(nextGeneration[individualIndex]);
                    }
                }
                else
                {
                    decimal fitVal;
                    if (GetFitnessVal(out fitVal))
                    {
                        nextGeneration[individualIndex].Fitness = fitVal;      //Set gene to slider and get fitness
                    }
                    else
                    {
                        nextGeneration[individualIndex].IsEnableFitness = false;
                    }
                    individualIndex++;
                    if (individualIndex < GeneAlgoOption.IndividualNum)
                    {
                        SetSlider(nextGeneration[individualIndex]);
                    }
                }
            }
            catch (Exception e)
            {
                //-------Error handling
                MessageBox.Show(e.Message);
                if (GeneAlgoOption.gaForm != null)
                {
                    WriteLogForm("GA is terminated because an Unexpected Error occurred");
                    GeneAlgoOption.gaForm.TransitionStopingState();
                }
                return;
            }
            Task.Run(() => GARun());    //Next Loop
        }

        //-------Initialize Generation as random
        private static void CreateRandomGeneration(out List<Individual> _generation)
        {
            _generation = new List<Individual>();
            for (int i = 0; i < GeneAlgoOption.IndividualNum; i++)
            {
                _generation.Add(new Individual());
                for (int j = 0; j < GeneAlgoOption.geneLength; j++)
                {
                    _generation.Last().AddGene((decimal)rnd.NextDouble());  //Fill one gene randomly
                }
            }
        }

        //-------Reflect the value in the slider
        private static void SetSlider(Individual _ind)
        {
            Grasshopper.Instances.DocumentEditor.Invoke(new Action(() =>
            {
                //-------Adapted to slider for each gene
                for (int i = 0; i < GeneAlgoOption.geneLength; i++)
                {
                    decimal sliMin = GeneAlgoOption.geneSliders[i].Slider.Minimum;
                    GeneAlgoOption.geneSliders[i].SetSliderValue(_ind.GetGene(i) * (GeneAlgoOption.geneSliders[i].Slider.Maximum - sliMin) + sliMin);
                }
                GeneAlgoOption.GhDoc.NewSolution(false);    //Recalculate uncalculated parts
            }));
        }

        //-------Acquire fitness from Param
        private static bool GetFitnessVal(out decimal _fitVal)
        {
            try
            {
                Grasshopper.Kernel.Types.GH_Number item = (Grasshopper.Kernel.Types.GH_Number)GeneAlgoOption.fitnessParam.VolatileData.get_Branch(0)[0];
                _fitVal = (decimal)item.Value;
                return true;
            }
            catch
            {
                _fitVal = 0M;
                return false;
            }
        }

        //-------Make the best individuals in the generation reflected in GH
        private static void SetExcellenceIndividual(List<Individual> _generation)
        {
            if (bestIndividual != null)
            {
                SetSlider(bestIndividual);
            }
            else
            {
                _generation = SortGeneration(_generation);  //Sort by excellence
                SetSlider(_generation[0]);
            }
        }

        //-------Output fitness to form log
        private static void OutputFitnessVal(List<Individual> _generation, bool _IsOverviewOnly)
        {
            var enableGeneration = _generation.Where(g => g.IsEnableFitness == true);
            if (_IsOverviewOnly) //Output summary value only
            {
                string outputText = "Fitness: Average[";
                outputText += enableGeneration.Average(g => g.Fitness).ToString("G6");  //6 significant digits
                outputText += "],Max[";
                outputText += enableGeneration.Max(g => g.Fitness).ToString("G6");
                outputText += "],Min[";
                outputText += enableGeneration.Min(g => g.Fitness).ToString("G6");
                outputText += "]";
                WriteLogForm(outputText);
            }
            else                 //Output fitness for all individuals
            {
                string outputText = "Fitness: [";
                foreach (Individual ind in enableGeneration)
                {
                    outputText += ind.Fitness.ToString() + ',';
                }
                outputText.TrimEnd(',');
                outputText += "]";
                WriteLogForm(outputText);
            }
        }

        //-------Draw fitness on form graph
        private static void DrawFitnessVal(List<Individual> _generation, bool _IsUpdateBestFitness)
        {
            var enableGeneration = _generation.Where(g => g.IsEnableFitness == true);
            GeneAlgoOption.gaForm.DrawChart(age, (double)enableGeneration.Average(g => g.Fitness), (double)enableGeneration.Max(g => g.Fitness), (double)enableGeneration.Min(g => g.Fitness), _IsUpdateBestFitness);
        }

        //-------Rearrange individuals in generations in order of excellence
        private static List<Individual> SortGeneration(List<Individual> _generation)
        {
            var enableGeneration = _generation.Where(g => g.IsEnableFitness == true);
            var sorted = (GeneAlgoOption.IsMinimize) ? enableGeneration.OrderBy(i => i.Fitness).ToList() : enableGeneration.OrderByDescending(i => i.Fitness).ToList();      //適応度順に降順ソート
            return sorted;
        }

        //-------Choose a roulette from within generation
        private static Individual SelectionIndividual(List<Individual> _generation)
        {
            var enableGeneration = _generation.Where(g => g.IsEnableFitness == true).ToList();
            decimal sumFitness = enableGeneration.Sum(g => g.Fitness);
            decimal arrow = (decimal)rnd.NextDouble() * sumFitness;
            decimal sum = 0.0M;
            foreach (Individual ind in enableGeneration)
            {
                sum += ind.Fitness;
                if (sum >= arrow)
                    return ind;
            }
            MessageBox.Show("SelectGeneFalse at Roulette");
            return enableGeneration[0];      //In case of error return 0th
        }

        //-------Normalize fitness within generation
        private static List<Individual> NormaliseFitnessGeneration(List<Individual> _generation)
        {
            List<Individual> normalisedGeneration = new List<Individual>();
            normalisedGeneration.AddRange(_generation.Select(g => (Individual)g.DeepCopyIndividual()));  //Deep copy
            var enableGeneration = normalisedGeneration.Where(g => g.IsEnableFitness == true).ToList();
            decimal maxFitness = enableGeneration.Max(g => g.Fitness);
            decimal minFitness = enableGeneration.Min(g => g.Fitness);
            decimal fitnessRange = maxFitness - minFitness;
            if (fitnessRange != 0)       //Prevent zero division when converging
            {
                if (GeneAlgoOption.IsMinimize)
                    normalisedGeneration.Where(g => g.IsEnableFitness == true).ToList().ForEach(g => g.Fitness = 1 - (g.Fitness - minFitness) / fitnessRange);
                else
                    normalisedGeneration.Where(g => g.IsEnableFitness == true).ToList().ForEach(g => g.Fitness = (g.Fitness - minFitness) / fitnessRange);
            }
            else
            {
                normalisedGeneration.Where(g => g.IsEnableFitness == true).ToList().ForEach(g => g.Fitness = 1.0M);
            }
            return normalisedGeneration;
        }

        //-------Crossover BLX-α
        private static Individual CrossoverGene_BLXa(Individual _parent1, Individual _parent2)
        {
            Individual child = new Individual();
            decimal alpha = 0.366M;                                                 //Recommended value in BLX-α
            for (int i = 0; i < GeneAlgoOption.geneLength; i++)
            {
                decimal r = (decimal)rnd.NextDouble() * (1 + 2 * alpha) - alpha;    //Blur range
                decimal nextGene = r * _parent1.GetGene(i) + (1 - r) * _parent2.GetGene(i);     //Individuals after crossover
                child.AddGene(nextGene);
            }
            return child;
        }

        //-------Mutations to individuals
        private static Individual MutationGene(Individual _ind, double _mutationRate)
        {
            for (int i = 0; i < GeneAlgoOption.geneLength; i++)
            {
                if (rnd.NextDouble() < _mutationRate)
                {
                    decimal normRnd = (decimal)(Math.Sqrt(-2 * Math.Log(rnd.NextDouble())) * Math.Cos(2 * Math.PI * rnd.NextDouble())); //Pseudo normal distribution(0,1) by the Box-Mueller method
                    decimal mutationedGene = _ind.GetGene(i) + 0.2M * normRnd;              //Mutations: Scaled the normal distribution by a factor of 0.2 to match the possible range of the gene value
                    _ind.SetGene(mutationedGene, i);
                }
            }
            return _ind;
        }

        //-------Save generation with CSV
        private static void OutpuCSVGeneration(List<Individual> _generation, bool _IsAdd)
        {
            string csvFilePath = GeneAlgoOption.SavePath;
            System.IO.StreamWriter sw = new System.IO.StreamWriter(csvFilePath, _IsAdd);
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            foreach (Individual ind in _generation)
            {
                sw.Write(age.ToString(nfi) + ',');                 //Age
                sw.Write(ind.Fitness.ToString(nfi) + ',');         //Fitness
                sw.Write(ind.IsEnableFitness.ToString(nfi) + ','); //Enable
                foreach (decimal gene in ind.GetGenes())
                {
                    sw.Write(gene.ToString(nfi) + ',');            //Genes
                }
                sw.Write("\r\n");
            }
            sw.Close();
        }

        //-------Read generation from CSV
        private static bool InputCSVGeneration(out List<Individual> _generation)
        {
            _generation = new List<Individual>();
            string csvFilePath = GeneAlgoOption.SavePath;
            if (!System.IO.File.Exists(csvFilePath))    //When the file can not be found
            {
                WriteLogForm("Not Find CSV File!");
                return false;
            }

            System.IO.StreamReader sr = new System.IO.StreamReader(csvFilePath, true);

            age = 0;
            int individualCount = 0;
            bool IsAgeReaded = false;

            try
            {
                while (sr.EndOfStream == false)
                {
                    Individual ind = new Individual();

                    string line = sr.ReadLine();
                    string[] fields = line.Split(',');
                    individualCount++;

                    if (fields.Length != GeneAlgoOption.geneLength + 4)  //Gene length is different from preserved
                    {
                        WriteLogForm("GeneLength is different from CSV File!");
                        return false;
                    }
                    if (!IsAgeReaded)
                    {
                        age = int.Parse(fields[0]);     //Read Age only once
                        IsAgeReaded = true;
                    }
                    ind.Fitness = decimal.Parse(fields[1]);         //Read Fitness
                    ind.IsEnableFitness = bool.Parse(fields[2]);    //Read Enable
                    for (int i = 3; i < fields.Length - 1; i++)
                    {
                        ind.AddGene(decimal.Parse(fields[i]));      //Read Gene
                    }
                    _generation.Add(ind);
                }
                if (individualCount != GeneAlgoOption.IndividualNum) //Number of individuals is different from preserved
                {
                    WriteLogForm("IndividualNum is different from CSV File!");
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
            GeneAlgoOption.gaForm.WriteLog(_logtext);
        }
    }
}