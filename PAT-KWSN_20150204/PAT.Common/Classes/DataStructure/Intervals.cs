using System.Collections.Generic;
using PAT.Common.Classes.Ultility;
//using System.Diagnostics.Contracts; /* chengbin */

namespace PAT.Common.Classes.DataStructure
{
    public class Intervals
    {
        public List<int> lowerbounds;
        public List<int> upperbounds;
       
        //chengbin: store old value
        public List<int> ol;
        public List<int> ou;

        public Intervals()
        {
            lowerbounds = new List<int>();
            upperbounds = new List<int>();
            //chengbin
            ol = new List<int>();
            ou = new List<int>();
        }

        public void AddInterval(int lower, int upper)
        {
            //chengbin
            //int oldLower = lower, oldUpper = upper;
            //System.Diagnostics.Debug.Assert(lower <= upper);
            //Contract.Ensures(Contract.ForAll(0, ol.Count, index => ContainsInterval(ol[index], ou[index])));
            //Contract.Ensures(ContainsInterval(lower, upper));
            //Contract.Ensures(Contract.ForAll(0, lowerbounds.Count, index => ContainsIntervalV2(ol, ou, oldLower, oldUpper, lowerbounds[index], upperbounds[index])));
            ol = lowerbounds;
            ou = upperbounds;

            if (lowerbounds.Count == 0)
            {
                lowerbounds.Add(lower);
                upperbounds.Add(upper);
                return;
            }

            bool selfupdate = false;
            int i = 0;
            while (i < lowerbounds.Count)
            {
                if (lower < lowerbounds[i])
                {
                    if (upper < lowerbounds[i])
                    {
                        if (!selfupdate)
                        {
                            lowerbounds.Insert(i, lower);
                            upperbounds.Insert(i, upper);
                        }
                        i = -1;
                        break;
                    }
                    else
                    {
                        lowerbounds[i] = lower;
                        if (upper > upperbounds[i])
                        {
                            upperbounds[i] = upper;
                            if (selfupdate)
                            {
                                lowerbounds.RemoveAt(i - 1);
                                upperbounds.RemoveAt(i - 1);
                            }
                            else
                            {
                                selfupdate = true;
                                //lower = lowerbounds[i];
                                //upper = upperbounds[i];
                                i++;
                            }

                        }
                        else
                        {
                            if (selfupdate)
                            {
                                lowerbounds.RemoveAt(i - 1);
                                upperbounds.RemoveAt(i - 1);
                            }
                            i = -1;
                            break;
                        }
                    }


                }
                else
                {
                    if (lower > upperbounds[i])
                    {
                        i++;
                    }
                    else
                    {
                        if (upper <= upperbounds[i])
                        {
                            if (selfupdate)
                            {
                                lowerbounds.RemoveAt(i - 1);
                                upperbounds.RemoveAt(i - 1);
                            }
                            i = -1;
                            break;
                        }
                        else
                        {
                            upperbounds[i] = upper;
                            lower = lowerbounds[i];
                            if (selfupdate)
                            {
                                //lower = lowerbounds[i];
                                //upper = upperbounds[i];

                                lowerbounds.RemoveAt(i - 1);
                                upperbounds.RemoveAt(i - 1);
                            }
                            else
                            {
                                selfupdate = true;
                                i++;
                            }

                        }
                    }
                }
            }

            if (i != -1 && lowerbounds[lowerbounds.Count - 1] != lower && upperbounds[upperbounds.Count - 1] != upper)
            {
                lowerbounds.Add(lower);
                upperbounds.Add(upper);
            }
        }

        public bool ContainsInterval(int lower, int upper)
        {
            for (int i = 0; i < lowerbounds.Count; i++)
            {
                if (lower >= lowerbounds[i])
                {
                    if (upperbounds[i] >= upper)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override string ToString()
        {
            string toString = "";
            for (int i = 0; i < lowerbounds.Count; i++)
            {
                toString += "[" + lowerbounds[i] + "," + (upperbounds[i] == int.MaxValue ? Constants.INFINITE : upperbounds[i].ToString()) + "] ";
            }
            return toString;
        }

        //chengbin
        public bool ContainsIntervalV2(List<int> lowbounds, List<int> upbounds, int low, int up, int lower, int upper)
        {
            int tempLow = 0, tempUp = 0;
            bool updateLow = false, updateUp = false;

            for (int i = 0; i < lowbounds.Count; i++)
            {
                if (lower >= lowbounds[i])
                {
                    if (upbounds[i] >= upper)
                    {
                        return true;
                    }
                }
            }

            if (lower >= low && upper <= up)
                return true;

            for (int i = 0; i < lowbounds.Count && !updateUp; i++)
            {
                if (low <= lowbounds[i])
                {
                    if (!updateLow)
                    {
                        tempLow = low;
                        updateLow = true;
                    }

                    if (!updateUp)
                    {
                        if (up <= lowbounds[i])
                        {
                            tempUp = up;
                            updateUp = true;
                        }
                        else
                        {
                            if (up <= upperbounds[i])
                            {
                                tempUp = upbounds[i];
                                updateUp = true;
                            }
                        }
                    }

                }
                else
                {
                    if (low <= upbounds[i])
                    {
                        tempLow = lowbounds[i];
                        updateLow = true;

                        if (up <= upbounds[i])
                        {
                            tempUp = upbounds[i];
                            updateUp = true;
                        }
                    }

                }
            }

            if (lower >= tempLow && upper <= tempUp)
                return true;
            else
                return false;
        }

        //chengbin
        //[ContractInvariantMethod]
        //protected void ObjectInvariant()
        //{
        //    Contract.Invariant(this.lowerbounds.Count == this.upperbounds.Count);
        //    Contract.Invariant(Contract.ForAll(0, this.lowerbounds.Count, index => this.lowerbounds[index] <= this.upperbounds[index]));
        //    Contract.Invariant(Contract.ForAll(0, this.lowerbounds.Count, index => (this.lowerbounds.Count == 0 || index == (this.lowerbounds.Count - 1) || (this.lowerbounds[index + 1] > this.upperbounds[index]))));
        //}

    }
}
