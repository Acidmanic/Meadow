using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acidmanic.Utilities.Reflection.ObjectTree;
using Meadow.Test.Functional.Search.Contracts;

namespace Meadow.Test.Functional.Search.Services
{
    public class IndexingService<TStorage>
    {


        private readonly ITransliterationService _transliterationService;

        public IndexingService(ITransliterationService transliterationService)
        {
            _transliterationService = transliterationService;
        }

        public string GetIndexCorpus(TStorage storage, bool fullTree)
        {
            var evaluator = new ObjectEvaluator(storage);

            var stringType = typeof(string);

            IEnumerable<AccessNode> textNodes =
                fullTree ? evaluator.Map.Nodes.Where(n => n.IsLeaf) : evaluator.RootNode.GetDirectLeaves();

            textNodes = textNodes.Where(n => n.Type == stringType);

            var sb = new StringBuilder();

            foreach (var textNode in textNodes)
            {
                var key = evaluator.Map.FieldKeyByNode(textNode);
            
                var text = evaluator.Read(key,true) as string ?? "";

                sb.Append(text).Append(" ");
            }

            var rawCorpus = sb.ToString();

            var indexCorpus = _transliterationService.Transliterate(rawCorpus);

            return indexCorpus;
        }
    }
}