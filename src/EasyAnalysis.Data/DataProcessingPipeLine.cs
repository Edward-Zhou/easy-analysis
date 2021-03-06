﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAnalysis.Data
{
    public class DataProcessingPipeLine<TRecord>
    {
        public class DataProcessingPipeLineContext
        {
            private DataProcessingPipeLine<TRecord> _pipeline;

            public DataProcessingPipeLineContext(DataProcessingPipeLine<TRecord> pipeline)
            {
                _pipeline = pipeline;
            }

            public void Output(IDictionary<string, object> record)
            {
                _pipeline.OnOutput(record);
            }
        }

        public event Action<IDictionary<string, object>> OnOutput;

        public Framework.Data.IReadOnlyCollection<TRecord> Source;

        private DataProcessingPipeLineContext _context;

        public DataProcessingPipeLine()
        {
            _context = new DataProcessingPipeLineContext(this);
        }

        public async Task Process(Func<DataProcessingPipeLineContext, TRecord, Task> processor)
        {
            await Source.ForEachAsync(async (item) => {

                await processor(_context, item);
            });
        }

        public async Task Process(Action<DataProcessingPipeLineContext, TRecord> processor)
        {
            await Source.ForEachAsync((item) =>
            {
                processor(_context, item);
            });
        }
    }
}
