#ifndef BT_NODE_H
#define BT_NODE_H

#include <string>
#include "General.h"

namespace fluentBehaviorTree
{
	class Node
	{
		std::string mName;
		bool mIsOpen;
		EStatus mResult;

	public:

		// 
		EStatus Tick();

	private:
		// Enter node and prepare for execution
		void Enter();

		/// <summary>
		/// Open node only if node has not been opened before
		/// </summary>
		virtual void Open();

		// Actual tick to be overriden by every child
		virtual EStatus tick() = 0;

		// Exit node after every tick
		void Exit();

		// Close node to ensure we don't go through it again
		void Close();
	};
} // namespace fluentBehaviorTree

#endif