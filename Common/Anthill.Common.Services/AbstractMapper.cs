using Anthill.Common.Services.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anthill.Common.Services
{
    public abstract class AbstractMapper<S, T> : IMapper<S, T>
        where S : class
        where T : class
    {
        public virtual T MapToModel(S source, Action<S, T> extra = null)
        {
            return MapToModel<T>(source, extra);
        }

        public virtual S MapFromModel(T model, Action<T, S> extra = null, S source = null)
        {
            return MapFromModel<S>(model, extra, source);
        }

        public virtual IEnumerable<T> MapCollectionToModel(IEnumerable<S> source, Action<S, T> extra = null)
        {
            return source.Select(x => MapToModel<T>(x, extra)).ToList();
        }

        public virtual TItem MapToModel<TItem>(S source, Action<S, TItem> extra = null)
            where TItem : T
        {
            var result = Mapper.Map<TItem>(source);

            extra?.Invoke(source, result);

            return result;
        }

        public virtual S MapFromModel<TItem>(T model, Action<T, TItem> extra = null, TItem source = null)
            where TItem : class, S
        {
            TItem result = null;

            if (source == null)
            {
                result = Mapper.Map<TItem>(model);
            }
            else
            {
                result = Mapper.Map<T, TItem>(model, source);
            }


            if (extra != null)
            {
                extra(model, result);
            }

            return result;
        }

        public virtual IEnumerable<TItem> MapCollectionToModel<TItem>(IEnumerable<S> source, Action<S, TItem> extra = null)
            where TItem : T
        {
            return source.Select(x => MapToModel<TItem>(x, extra)).ToList();
        }

        public virtual IEnumerable<S> MapCollectionFromModel<TItem>(IEnumerable<T> model, Action<T, TItem> extra = null, TItem source = null)
            where TItem : class, S
        {
            return model.Select(x => MapFromModel<TItem>(x, extra)).ToList();
        }

        protected abstract AutoMapper.IMapper Configure();

        public IEnumerable<T> MapCollectionToModel(IEnumerable source, Action<S, T> extra = null)
        {
            var result = new List<T>();
            foreach (var item in source)
            {
                result.Add(MapToModel<T>((S)item, extra));
            }

            return result;
        }

        protected AutoMapper.IMapper Mapper
        {
            get
            {
                if (_mapper == null)
                {
                    _mapper = Configure();
                }

                return _mapper;
            }
        }

        private AutoMapper.IMapper _mapper;
    }
}
