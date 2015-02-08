using System;
using System.Collections.Generic;

namespace PAT.Common.Classes.SemanticModels.MDP
{
    public class SingleSamplingPlan
    {
        public long minSmpSize;
        public long minTruthSize;

        public SingleSamplingPlan(long X, long Y)
        {
            minSmpSize = X;
            minTruthSize = Y;
        }

        public void SetminSS(long X)
        {
            minSmpSize = X;
        }

        public void SetminTS(long Y)
        {
            minTruthSize = Y;
        }

        public static double logFactorial(long n)
        {
            if (n > 254)
            {
                double x = n + 1;
                return (x - 0.5) * Math.Log(x) - x + 0.5 * Math.Log(2 * Math.PI) + 1.0 / (12.0 * x);
            }

            double[] lf =
                {
                    0.000000000000000,
                    0.000000000000000,
                    0.693147180559945,
                    1.791759469228055,
                    3.178053830347946,
                    4.787491742782046,
                    6.579251212010101,
                    8.525161361065415,
                    10.604602902745251,
                    12.801827480081469,
                    15.104412573075516,
                    17.502307845873887,
                    19.987214495661885,
                    22.552163853123421,
                    25.191221182738683,
                    27.899271383840894,
                    30.671860106080675,
                    33.505073450136891,
                    36.395445208033053,
                    39.339884187199495,
                    42.335616460753485,
                    45.380138898476908,
                    48.471181351835227,
                    51.606675567764377,
                    54.784729398112319,
                    58.003605222980518,
                    61.261701761002001,
                    64.557538627006323,
                    67.889743137181526,
                    71.257038967168000,
                    74.658236348830158,
                    78.092223553315307,
                    81.557959456115029,
                    85.054467017581516,
                    88.580827542197682,
                    92.136175603687079,
                    95.719694542143202,
                    99.330612454787428,
                    102.968198614513810,
                    106.631760260643450,
                    110.320639714757390,
                    114.034211781461690,
                    117.771881399745060,
                    121.533081515438640,
                    125.317271149356880,
                    129.123933639127240,
                    132.952575035616290,
                    136.802722637326350,
                    140.673923648234250,
                    144.565743946344900,
                    148.477766951773020,
                    152.409592584497350,
                    156.360836303078800,
                    160.331128216630930,
                    164.320112263195170,
                    168.327445448427650,
                    172.352797139162820,
                    176.395848406997370,
                    180.456291417543780,
                    184.533828861449510,
                    188.628173423671600,
                    192.739047287844900,
                    196.866181672889980,
                    201.009316399281570,
                    205.168199482641200,
                    209.342586752536820,
                    213.532241494563270,
                    217.736934113954250,
                    221.956441819130360,
                    226.190548323727570,
                    230.439043565776930,
                    234.701723442818260,
                    238.978389561834350,
                    243.268849002982730,
                    247.572914096186910,
                    251.890402209723190,
                    256.221135550009480,
                    260.564940971863220,
                    264.921649798552780,
                    269.291097651019810,
                    273.673124285693690,
                    278.067573440366120,
                    282.474292687630400,
                    286.893133295426990,
                    291.323950094270290,
                    295.766601350760600,
                    300.220948647014100,
                    304.686856765668720,
                    309.164193580146900,
                    313.652829949878990,
                    318.152639620209300,
                    322.663499126726210,
                    327.185287703775200,
                    331.717887196928470,
                    336.261181979198450,
                    340.815058870798960,
                    345.379407062266860,
                    349.954118040770250,
                    354.539085519440790,
                    359.134205369575340,
                    363.739375555563470,
                    368.354496072404690,
                    372.979468885689020,
                    377.614197873918670,
                    382.258588773060010,
                    386.912549123217560,
                    391.575988217329610,
                    396.248817051791490,
                    400.930948278915760,
                    405.622296161144900,
                    410.322776526937280,
                    415.032306728249580,
                    419.750805599544780,
                    424.478193418257090,
                    429.214391866651570,
                    433.959323995014870,
                    438.712914186121170,
                    443.475088120918940,
                    448.245772745384610,
                    453.024896238496130,
                    457.812387981278110,
                    462.608178526874890,
                    467.412199571608080,
                    472.224383926980520,
                    477.044665492585580,
                    481.872979229887900,
                    486.709261136839360,
                    491.553448223298010,
                    496.405478487217580,
                    501.265290891579240,
                    506.132825342034830,
                    511.008022665236070,
                    515.890824587822520,
                    520.781173716044240,
                    525.679013515995050,
                    530.584288294433580,
                    535.496943180169520,
                    540.416924105997740,
                    545.344177791154950,
                    550.278651724285620,
                    555.220294146894960,
                    560.169054037273100,
                    565.124881094874350,
                    570.087725725134190,
                    575.057539024710200,
                    580.034272767130800,
                    585.017879388839220,
                    590.008311975617860,
                    595.005524249382010,
                    600.009470555327430,
                    605.020105849423770,
                    610.037385686238740,
                    615.061266207084940,
                    620.091704128477430,
                    625.128656730891070,
                    630.172081847810200,
                    635.221937855059760,
                    640.278183660408100,
                    645.340778693435030,
                    650.409682895655240,
                    655.484856710889060,
                    660.566261075873510,
                    665.653857411105950,
                    670.747607611912710,
                    675.847474039736880,
                    680.953419513637530,
                    686.065407301994010,
                    691.183401114410800,
                    696.307365093814040,
                    701.437263808737160,
                    706.573062245787470,
                    711.714725802289990,
                    716.862220279103440,
                    722.015511873601330,
                    727.174567172815840,
                    732.339353146739310,
                    737.509837141777440,
                    742.685986874351220,
                    747.867770424643370,
                    753.055156230484160,
                    758.248113081374300,
                    763.446610112640200,
                    768.650616799717000,
                    773.860102952558460,
                    779.075038710167410,
                    784.295394535245690,
                    789.521141208958970,
                    794.752249825813460,
                    799.988691788643450,
                    805.230438803703120,
                    810.477462875863580,
                    815.729736303910160,
                    820.987231675937890,
                    826.249921864842800,
                    831.517780023906310,
                    836.790779582469900,
                    842.068894241700490,
                    847.352097970438420,
                    852.640365001133090,
                    857.933669825857460,
                    863.231987192405430,
                    868.535292100464630,
                    873.843559797865740,
                    879.156765776907600,
                    884.474885770751830,
                    889.797895749890240,
                    895.125771918679900,
                    900.458490711945270,
                    905.796028791646340,
                    911.138363043611210,
                    916.485470574328820,
                    921.837328707804890,
                    927.193914982476710,
                    932.555207148186240,
                    937.921183163208070,
                    943.291821191335660,
                    948.667099599019820,
                    954.046996952560450,
                    959.431492015349480,
                    964.820563745165940,
                    970.214191291518320,
                    975.612353993036210,
                    981.015031374908400,
                    986.422203146368590,
                    991.833849198223450,
                    997.249949600427840,
                    1002.670484599700300,
                    1008.095434617181700,
                    1013.524780246136200,
                    1018.958502249690200,
                    1024.396581558613400,
                    1029.838999269135500,
                    1035.285736640801600,
                    1040.736775094367400,
                    1046.192096209724900,
                    1051.651681723869200,
                    1057.115513528895000,
                    1062.583573670030100,
                    1068.055844343701400,
                    1073.532307895632800,
                    1079.012946818975000,
                    1084.497743752465600,
                    1089.986681478622400,
                    1095.479742921962700,
                    1100.976911147256000,
                    1106.478169357800900,
                    1111.983500893733000,
                    1117.492889230361000,
                    1123.006317976526100,
                    1128.523770872990800,
                    1134.045231790853000,
                    1139.570684729984800,
                    1145.100113817496100,
                    1150.633503306223700,
                    1156.170837573242400,
                };
                return lf[n];
           
        }

        public static double coeflog(long n, long k)
        {

            return Math.Exp(logFactorial(n) - logFactorial(n - k)
                        - logFactorial(k));
        }

        public static double coef(long n, long k)
        {
            long result = 1;

            for (long i = Math.Max(k, n - k) + 1; i <= n; ++i)
                result *= i;


            for (long i = 2; i <= Math.Min(k, n - k); ++i)
                result /= i;

            return result;
        }

        public static double normalPdf(double x)
        {
            x = -(x * x) / 2;
            double normDist = (1 / Math.Sqrt(2 * Math.PI)) * (Math.Exp(x));
            return normDist;
        }

        public static double binomialPdf(long successes, long trials, double probabilityOfSuccess)
        {
            double u = trials * probabilityOfSuccess;
            double std = Math.Sqrt(u * (1 - probabilityOfSuccess));

            //from wiki about binominal pdf for large value N
            //it is an approximation of normal distribution with N(np, np(1-p))
            if ((u > 1000) && (trials * (1 - probabilityOfSuccess) > 1000) && (std != 0))
            {
                return (normalPdf((successes - u) / std)) / std;
            }

            double probOfFailures = 1 - probabilityOfSuccess;

            if (successes > Math.Floor((decimal)trials / 2))
                return binomialPdf(trials - successes, trials, 1 - probabilityOfSuccess);

                //double c = coeflog(trials, successes);
                //double px = Math.Pow(probabilityOfSuccess, successes);
                //double qnx = Math.Pow(probOfFailures, trials - successes);

                //return c * px * qnx;
            return coeflog(trials, successes)*Math.Pow(probabilityOfSuccess, successes)*
                   Math.Pow(probOfFailures, trials - successes);
        }

        public static double binomialCdf(long successes, long trials, double probabilityOfSuccess)
        {
            double c = 0;
            long counter = 0;
            while (counter <= successes)
            {
                c = c + binomialPdf(counter, trials, probabilityOfSuccess);
                counter++;
            }
            return c;
        }

        public static long binoiaCdfInvs(double a, long N, double probability)
        {
            double value = 1;
            long i = N;
            while (value >= a)
            {
                i = i - 1;
                value = binomialCdf(i, N, probability);
            }
            return i;

        }

        public void SSPAlgGeneral(double p0, double p1, double alpha, double beta)
        {
            long nmin = 1, nmax = -1;
            long n = nmin;
            while (((nmax < 0) || (nmin < nmax)) && (n > 0))
            {
                long x0 = binoiaCdfInvs(alpha, n, p0);
                long x1 = binoiaCdfInvs(1 - beta, n, p1);
                if ((x0 >= x1) && (x0 >= 0))
                {
                    nmax = n;
                }
                else
                {
                    nmin = n + 1;
                }

                if (nmax < 0)
                {
                    n = 2*n;
                }
                else
                {
                    n = (long) Math.Floor((double) ((nmin + nmax)/2));
                }
            } //end of while

            n = nmax;
            long c0 = binoiaCdfInvs(alpha, n, p0);

            long c1 = binoiaCdfInvs(1 - beta, n, p1);
            while (c0 <= c1)
            {
                n = n + 1;
                c0 = binoiaCdfInvs(alpha, n, p0);
                c1 = binoiaCdfInvs(1 - beta, n, p1);
            }

            var c = (long) Math.Ceiling((c0 + c1 + 0.5)/2);
            SetminSS(n);
            SetminTS(c);
        }

        public void SSPAlgSpefic(double theta, double sigma, double alpha, double beta)
        {
            SSPAlgGeneral(theta + sigma, theta - sigma, alpha, beta);

        }

    }

    public class Simulation
    {
        public MDPStat mdp;
        public Policy pl;
        public long trials;
        public long success;
        public bool singleTrial;
        public bool status;
        public Random random;
        //status=true means: single trial, otherwise, mutliple trails


        public Simulation(MDPStat mdpl)
        {
            mdp = mdpl;
            random = new Random();
        }

        public Simulation(long Y)
        {
            trials = Y;
            status = true;
            random = new Random();
        }

        public Simulation(long Y, Random random)
        {
            trials = Y;
            status = true;
            this.random = random;
        }

        public void setModel(MDPStat mdpstat)
        {
            mdp = mdpstat;
        }

        public void setStatus(bool s)
        {
            status = s;
        }

        public long getSuccess()
        {
            success = simulationCollection(trials);
            return success;
        }

        public bool getsingleTrial()
        {
            return testState(mdp.InitState, mdp.getTargetStates(), mdp.NoneZeroStates);
        }

        public MDPStateStat sampleNextState(DistributionStat action)
        {
            double value = random.NextDouble();
            double accumulation = 0;
         
            foreach (KeyValuePair<double, MDPStateStat> pair in action.States)  //assume every state has distribution(s) with at least one pair
            {
                double preaccumulation = accumulation;
                accumulation += pair.Key;
                if (value > preaccumulation && value <= accumulation) return pair.Value;
            }
            return null;
        }

        public bool testState(MDPStateStat ss, List<MDPStateStat> targetSet, HashSet<MDPStateStat> NoneZeroStates)
        {
            if (targetSet.Contains(ss)) return true;
            if (!NoneZeroStates.Contains(ss)) return false;
            if (ss.Distributions.Count==0) {return false;}
            return testState(sampleNextState(ss.action), targetSet, NoneZeroStates);
        }

        public bool simulationSingle()
        {
            return getsingleTrial();
        }

        public long simulationCollection(long upper)
        {
            long result = 0;
            for (int i = 0; i < upper; ++i)
            {
                if (simulationSingle())
                { result++; }
            }
            return result;
        }
    }

    public class RunTest
    {
        public SingleSamplingPlan ssp;

        public bool SPRTresult;
        public long sprtcount;
        public long sprttotal;

        //SRT  parameters used in SRT algorithm and GUI_"Start a trail"
        public double sprtp0;
        public double sprtp1;
        public double sprtb1;
        public double sprtb2;
        public double sprta;
        private double sprtb;

        public bool chooseSamplingPlan = true;
        //private double sprtp0 { get; set; }
        //private double sprtp1 { get; set; }
        //private double sprtb1 { get; set; }
        //private double sprtb2 { get; set; }
        //private double sprta { get; set; }
        //private double sprtb { get; set; }

        //SRT parameters used in GUI_"Start a trail"
        public double sprtfm;
        public long sprtm;

        public double alpha;
        public double beta;
        public double sigma;
        public double theta;

        public void SetSSP(SingleSamplingPlan sspll)
        {
            ssp = sspll;
        }

        public void SetSamplingPlan(bool status)
        {
            chooseSamplingPlan = status;  //true is for SPRT plan, flase is for the SSP;
        }

        public void initRunTest(double Alpha, double Beta, double Sigma, double Theta)
        {
            alpha = Alpha;
            beta = Beta;
            sigma = Sigma;
            theta = Theta;

            sprtp0 = theta + sigma;
            sprtp1 = theta - sigma;
            sprta = alpha;
            sprtb = beta;
            sprtb1 = Math.Log(sprtb / (1 - sprta));
            sprtb2 = Math.Log((1 - sprtb) / sprta);
            sprtfm = 0;
            sprtm = 0;
            sprtcount = 0;
            sprttotal = 0;
        }

        public void reSetRunTest()
        {
            sprtcount = 0;
            sprttotal = 0;
        }
        //Verify CTMC: sequential single sampling(SSP)  testrequires to calculate fixed sample size
        public bool singleSamplingTest(Simulation sml)
        {
            //ssp.SSPAlgSpefic(paraStat.theta, paraStat.sigma, paraStat.alpha, paraStat.beta);
            sml.trials = ssp.minSmpSize;
            long m = 0, dm = 0;
            while (dm <= ssp.minTruthSize && (dm + ssp.minSmpSize - m) > ssp.minTruthSize)
            {
                m++;
                if (sml.getsingleTrial())
                {
                    dm++;
                }
            }
            sprttotal = m;
            sprtcount = dm;
            return dm > ssp.minTruthSize ? true : false;
        }

        //Verify CTMC: sequential probability ratio sampling test(SPRST) doesn't require to calculate fixed sample size: on the fly
        public bool sequentialProbRatioTest(Simulation sml)
        {
            double p0 = theta + sigma;
            double p1 = theta - sigma;
            double a = alpha;
            double b = beta;

            if (p0 == 1 && p1 == 0)
            {
                // ssp.SSPAlgSpefic(paraStat.beta, paraStat.sigma, paraStat.alpha, paraStat.beta);
                return singleSamplingTest(sml);
            }
            long m = 0;
            double fm = 0;
            double bound1 = Math.Log((b / (1 - a)));
            double bound2 = Math.Log((1 - b) / a);
            while (fm > bound1 && fm < bound2)
            {
                m++;
                long dm;
                if (sml.getsingleTrial())
                {
                    dm = 1;
                    sprtcount++;
                }
                else
                {
                    dm = 0;
                }
                fm += dm * Math.Log(p1 / p0) + (1 - dm) * Math.Log((1 - p1) / (1 - p0));
            }
            sprttotal = m;
            return fm <= bound1 ? true : false;
            //if false, that means fm>=bound2(notice while loop)
        }

        //Verify DTMC: SSP
        public bool verifyMDPSSP(Simulation sml, Policy policy, out string report)
        {
            report = null;
            policy.reSetPlan();
            while (policy.hasPolicy())
            {
                reSetRunTest();
                report += policy.reportAdversary(policy.updatePolicy());
                bool result = singleSamplingTest(sml);
                string st = result ? "Hypothesis Accepted!" : "Hypothesis Rejected!";
                report += "\nSingle Sampling Plan Result:\n" + sprtcount + "/" + sprttotal + "\n" + st;
                if (!result) return false;
            }
            return true;
        }

        //Verify DTMC: SPRT
        public bool verifyMDPSPRT(Simulation sml, Policy policy, out string report)
        {
            report = null;
            policy.reSetPlan();
            while (policy.hasPolicy())
            {
                reSetRunTest();
                report += policy.reportAdversary(policy.updatePolicy());
                bool result = sequentialProbRatioTest(sml);

             // following two statements only used for 
                string st = result ? "Hypothesis Accepted!" : "Hypothesis Rejected!";
                report += "\nSeqential Ration Plan Result:\n" + sprtcount + "/" + sprttotal + "\n" + st;
                if (!result) return false;
            }
            return true;
        }
    }
}
