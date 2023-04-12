using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Irvin.Parser
{
    public class TokenCollection
    {
        private int _currentIndex;
        private readonly List<Token> _tokens;
        private readonly Stack<int> _checkpoints;

        public TokenCollection()
        {
            _currentIndex = -1;
            _tokens = new List<Token>();
            _checkpoints = new Stack<int>();
        }

        public TokenCollection(IEnumerable<Token> source)
            : this()
        {
            _tokens.AddRange(source);
        }

        public Token Current
        {
            get
            {
                if (_currentIndex < 0 || _currentIndex >= _tokens.Count)
                {
                    return null;
                }
                return _tokens[_currentIndex];
            }
        }

        internal void Add(Token token)
        {
            _tokens.Add(token);
        }

        public TokenCollection MoveNext()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException();
            }

            _currentIndex++;
            return this;
        }

        public bool HasNext()
        {
            return _currentIndex < _tokens.Count - 1;
        }

        public TokenCollection MoveNextSkippingSpaces()
        {
            return MoveAndSkip(x => x.IsSpace);
        }

        /// <summary>
        /// Ends on a non-space token or the last token in the collection
        /// </summary>
        /// <param name="receiver">the action to take on each token that gets skipped</param>
        /// <returns>A reference to this collection</returns>
        public TokenCollection MoveNextSkippingSpaces(Action<Token> receiver)
        {
            return MoveAndSkip(x => x.IsSpace, receiver);
        }

        public TokenCollection MoveNextSkippingSpaces(List<Token> receiver)
        {
            return MoveAndSkip(x => x.IsSpace, receiver.Add);
        }

        /// <summary>
        /// End on a non-delimiter token or the last token in the collection
        /// </summary>
        /// <param name="receiver">the action to take on each token that gets skipped</param>
        /// <returns>A reference to this collection</returns>
        public TokenCollection MoveNextSkippingDelimiters(Action<Token> receiver = null)
        {
            return MoveAndSkip(x => x.IsDelimiter, receiver);
        }

        public TokenCollection MoveNextSkippingNonSpaceDelimiters()
        {
            return MoveAndSkip(x => x.IsDelimiter && !Current.IsSpace);
        }

        public TokenCollection MoveNextSkippingSpacesAndNewLine(Action<Token> receiver = null)
        {
            return MoveAndSkip(x => x.IsSpace || x.Content.Equals(Environment.NewLine), receiver);
        }

        private TokenCollection MoveAndSkip(Func<Token, bool> itemToSkipEvaluator, Action<Token> receiver = null)
        {
            if (HasNext())
            {
                MoveNext();
            }

            while (HasNext() && itemToSkipEvaluator(Current))
            {
                receiver?.Invoke(Current);
                MoveNext();
            }

            return this;
        }

        public ReadOnlyCollection<Token> MoveUntil(Func<Token, bool> predicate)
        {
            IList<Token> tokens = new List<Token>();

            bool noMoreElements = false;

            if (_currentIndex == -1)
            {
                MoveNext();
            }
            if (Current == null)
            {
                noMoreElements = true;
            }

            while (!noMoreElements && !predicate(Current))
            {
                tokens.Add(Current);

                if (HasNext())
                {
                    MoveNext();
                }
                else
                {
                    noMoreElements = true;
                }
            }

            return new ReadOnlyCollection<Token>(tokens);
        }

        public Token PeekNext()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException();
            }

            return _tokens[_currentIndex + 1];
        }

        public IEnumerable<Token> ReadToEnd()
        {
            while (HasNext())
            {
                yield return Current;
                MoveNext();
            }
        }

        public void SetCheckpoint()
        {
            if (_currentIndex < 0)
            {
                MoveNext();
            }

            _checkpoints.Push(_currentIndex);
        }

        public void Rewind()
        {
            _currentIndex = _checkpoints.Pop();
        }

        public bool RewindIfPossible()
        {
            if (_checkpoints.Any())
            {
                Rewind();
                return true;
            }

            return false;
        }
    }
}