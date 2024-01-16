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

        public TokenCollection(IEnumerable<Token> tokens)
            : this()
        {
            _tokens.AddRange(tokens);
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

        public TokenCollection EnsureReady()
        {
            if (_currentIndex == -1 && _tokens.Count > 0)
            {
                MoveNext();
            }
            
            return this;
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
        
        public TokenCollection MoveToNextNonWhiteSpace()
        {
            return MoveToNextNonWhiteSpace(out _);
        }
        
        public TokenCollection MoveToNextNonWhiteSpace(out List<Token> whitespace)
        {
            MoveNext();
            whitespace = new List<Token>(MoveUntil(x => !x.IsWhitespace));
            return this;
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

        /// <summary>
        /// Iterates through the collection until finding the token that does not match the predicate. 
        /// Ends on the first token that does not match, or the end of the collection.
        /// </summary>
        /// <param name="predicate">any valid token conditional</param>
        /// <returns>
        /// The tokens that were iterated through, except the last one.
        /// (If not matching token was found and the collection was exhausted, the returned list contains everything iterated over.)
        /// </returns>
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

        /// <summary>
        /// The same as <see cref="MoveUntil"/>, except the current token is also returned.
        /// </summary>
        /// <param name="predicate">any valid token conditional</param>
        /// <returns>The tokens that were iterated through, including the last one OR all remaining tokens if predicate never matched.</returns>
        public ReadOnlyCollection<Token> MoveUntilInclusive(Func<Token, bool> predicate)
        {
            ReadOnlyCollection<Token> captures = MoveUntil(predicate);
            return new ReadOnlyCollection<Token>(captures.Union(new [] { Current }).ToList());
        }

        public Token PeekNext()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException();
            }

            return _tokens[_currentIndex + 1];
        }

        public void SetCheckpoint()
        {
            if (_currentIndex < 0)
            {
                MoveNext();
            }

            _checkpoints.Push(_currentIndex);
        }

        /// <summary>
        /// Reverts the collection back to the most recent checkpoint
        /// </summary>
        /// <exception cref="InvalidOperationException">if no checkpoints currently set</exception>
        public void Rewind()
        {
            _currentIndex = _checkpoints.Pop();
        }

        /// <summary>
        /// Converts the entire collection to IEnumerable (from start to end, not current to end)
        /// </summary>
        /// <returns>enumerable object</returns>
        public IEnumerable<Token> ToEnumerable()
        {
            return new List<Token>(_tokens);
        }

        public IEnumerable<Token> Remaining()
        {
            if (_currentIndex == -1)
            {
                MoveNext();
            }
            
            if (_currentIndex < _tokens.Count)
            {
                return _tokens.GetRange(_currentIndex, _tokens.Count - _currentIndex);
            }

            return null;
        }
    }
}