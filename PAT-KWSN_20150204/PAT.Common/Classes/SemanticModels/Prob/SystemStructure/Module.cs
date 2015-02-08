using System.Collections.Generic;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{
    public class Module
    {
        public string Name;
        public List<Command> Commands;
        public List<VarDeclaration> LocalDeclarations;

        public Module(string name, List<Command> commands, List<VarDeclaration> decls)
        {
            Name = name;
            Commands = commands;
            LocalDeclarations = decls;
        }

        public HashSet<string> GetAllSynchs()
        {
            HashSet<string> allSyncs = new HashSet<string>();
            foreach (var command in Commands)
            {
                if (command.Synch != string.Empty)
                {
                    allSyncs.Add(command.Synch);
                }
            }

            return allSyncs;
        }

        /// <summary>
        /// Create renamed module from an old one.
        /// At the module level, we just rename the local variable to create new variable
        /// At the SystemDef step, we will use all of the information to rename the encoding
        /// </summary>
        /// <param name="name"></param>
        /// <param name="renameMapping"></param>
        /// <returns></returns>
        public Module Rename(string name, Dictionary<string, string> renameMapping)
        {
            Module result = new Module(name, new List<Command>(), new List<VarDeclaration>());

            foreach (var localDeclaration in LocalDeclarations)
            {
                string localVarName = renameMapping.ContainsKey(localDeclaration.Name) ? renameMapping[localDeclaration.Name] : localDeclaration.Name;
                
                result.LocalDeclarations.Add(new VarDeclaration(localVarName, localDeclaration.Low, localDeclaration.High, localDeclaration.Init));

            }

            Dictionary<string, Expression> renameMappingExp = new Dictionary<string, Expression>();
            foreach (var pair in renameMapping)
            {
                renameMappingExp.Add(pair.Key, new Variable(pair.Value));
            }

            foreach (var command in Commands)
            {
                string synch = (renameMapping.ContainsKey(command.Synch)) ? renameMapping[command.Synch] : command.Synch;
                Expression guard = command.Guard.ClearConstant(renameMappingExp);

                List<Update> updates = new List<Update>();

                foreach (var update in command.Updates)
                {
                    List<Assignment> assignments = new List<Assignment>();

                    foreach (var assignment in update.Assignments)
                    {
                        assignments.Add(assignment.ClearConstant(renameMappingExp) as Assignment);
                    }

                    updates.Add(new Update(assignments, update.Probability));
                }

                result.Commands.Add(new Command(synch, guard, updates));
            }

            return result;
        }
    }
}
