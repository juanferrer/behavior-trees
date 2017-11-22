#include <string>

#include "Node.h"
#include "General.h"

namespace fluentBehaviorTree
{
	class BehaviorTree
	{
	private:
		std::string mName;
		Node* mRoot;

	public:

		BehaviorTree(std::string name, Node& root);

		std::string getName() { return mName; }

		// Tick tree to navigate and get result
		EStatus tick();


	protected:
		void setName(std::string name) { mName = name; }
	};
}