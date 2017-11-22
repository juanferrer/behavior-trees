#ifndef BT_NODE_H
#define BT_NODE_H

#include <string>
#include "General.h"

namespace fluentBehaviorTree
{
	class Node
	{
	private:
		std::string mName;
		bool mIsOpen;
		EStatus mResult;

		// Enter node and prepare for execution
		void enter();

		// Exit node after every tick
		void exit();

		// Close node to ensure we don't go through it again
		void close();

	public:

		// 
		EStatus tick();
		std::string getName() { return mName; }
		bool isOpen() { return mIsOpen; }
		EStatus getResult() { return mResult; }
		void setResult(EStatus result) { mResult = result; }

	protected:
		void setName(std::string name) { mName = name; }
		bool setOpen(bool isOpen) { mIsOpen = isOpen; }

		// Open node only if node has not been opened before
		virtual void open();

		// Actual tick to be overriden by every child
		virtual EStatus tickNode() = 0;

	};
}

#endif